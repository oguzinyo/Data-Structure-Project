using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures;

public class DirectedGraph : IGraph
/*Bu sınıf, blokzincir ağını modellemek için kullanılan temel yönlü graf (directed graph) veri yapısının saf implementasyonudur. 
Harici hazır koleksiyon kütüphanelerine bağımlı kalmadan; projeye özgü yazılmış karma tablo (HashTable) ve doğrusal bellek yapıları 
(UmmetQueue, UmmetStack) kullanılarak sıfırdan inşa edilmiştir. Çekirdek katmandaki IGraph sözleşmesini (interface) uygular.*/
{
    private readonly HashTable<string, WalletNode> _wallets = new(hashFunc: WalletHashFunctions.HashFNV1a);
    /*_wallets: Cüzdan adreslerini (string tabanlı) doğrudan WalletNode nesnelerine eşleyen ana karma tablodur. Hızlı erişim için yüksek dağılımlı FNV-1a özet (hash) algoritmasını kullanır.*/
    private readonly HashTable<string, List<TransactionEdge>> _adjacency = new(hashFunc: WalletHashFunctions.HashFNV1a);
    /*_adjacency: Grafın belkemiği olan Komşuluk Listesi (Adjacency List) yapısıdır. 
    Bir cüzdan adresini anahtar olarak alıp, o cüzdandan çıkan tüm transfer (TransactionEdge) kenarlarının listesini tutar. İki boyutlu matrislere kıyasla bellekteki uzay karmaşıklığını O(V + E) seviyesine düşürür.*/
    private readonly List<string> _addresses = new();
    /*_addresses: Ağdaki tüm cüzdan adreslerini basit bir doğrusal liste (List) içerisinde saklar. Sadece gelen işlemleri (incoming) ararken tüm grafı baştan sona taramak gerektiğinde (iterasyon amacıyla) kullanılır.*/

    public void BatuhanAddVertex(WalletNode wallet)
    {
    /*BatuhanAddVertex: Grafa yeni bir cüzdan düğümü ekler. Eklenen düğümü hem _wallets tablosuna, hem komşuluk listesine, hem de genel adres listesine eşzamanlı olarak kaydeder.*/
        if (!_wallets.ContainsKey(wallet.Address))
        {
            _wallets.Add(wallet.Address, wallet);
            _adjacency.Add(wallet.Address, new List<TransactionEdge>());
            _addresses.Add(wallet.Address);
        }
    }

    public void BatuhanAddEdge(TransactionEdge transaction)
    {
    /*BatuhanAddEdge: İki düğüm arasına yönlü bir transfer kenarı ekler. En kritik mimari özelliği mikro düzeyde kilit (Fine-Grained Locking) kullanmasıdır. 
    Transfer gerçekleşirken graf yapısının tamamını kilitlemek yerine, işlemciyi yormamak adına yalnızca hedef ve kaynak cüzdanların kendi içindeki BalanceLock kilitlerini devreye sokarak bakiyeleri günceller.*/
        BatuhanAddVertexIfMissing(transaction.FromAddress);
        BatuhanAddVertexIfMissing(transaction.ToAddress);

        _adjacency[transaction.FromAddress].Add(transaction);

        // Yeni modelde ApproximateBalance yerine doğrudan Balance kullanıyoruz
        lock (_wallets[transaction.FromAddress].BalanceLock)
        {
            _wallets[transaction.FromAddress].Balance -= transaction.Amount;
        }

        lock (_wallets[transaction.ToAddress].BalanceLock)
        {
            _wallets[transaction.ToAddress].Balance += transaction.Amount;
        }
    }

    public IReadOnlyList<string> BatuhanGetAddresses() => _addresses;
    /*BatuhanGetAddresses: Graf içindeki tüm cüzdan adreslerini tutan doğrusal listeyi (List) sadece okunabilir bir arayüzle (IReadOnlyList) dışarıya sunar.
    Bu güvenlik önlemi, dış katmanların grafın temel verisini manipüle etmesini (örneğin listeden eleman silmesini) engeller.*/

    public IReadOnlyList<TransactionEdge> BatuhanGetOutgoingTransactions(string address)
    {
    /*BatuhanGetOutgoingTransactions: Verilen bir cüzdan adresinin komşuluk listesinde yer alan giden transferlerini döndürür. 
    Eğer aranan cüzdan adresi karma tabloda (HashTable) yoksa, uygulamanın null referans (NullReferenceException) hatası verip çökmesini önlemek için bellekte yer kaplamayan boş bir dizi (Array.Empty) döndürür.*/
        if (!_adjacency.TryGetValue(address, out var transactions))
        {
            return Array.Empty<TransactionEdge>();
        }

        return transactions;
    }

    public decimal BatuhanGetApproximateBalance(string address)
    {
    /*BatuhanGetApproximateBalance: Ana karma tabloda (wallets) arama yaparak istenen cüzdanın mevcut bakiyesini döndürür. Cüzdan graf üzerinde henüz oluşturulmamışsa, okuma işleminin güvenli bir şekilde sonuçlanması için 0m (sıfır) döndürür.*/
        return _wallets.TryGetValue(address, out var wallet) ? wallet.Balance : 0m;
    }

    public decimal BatuhanGetIncomingTotal(string address)
    {
    /*BatuhanGetIncomingTotal: Belirli bir cüzdana dışarıdan gelen toplam parayı veya transfer kenarlarını bulur.
    Grafın mimarisi sadece "çıkan (outgoing)" yönleri komşuluk listesinde sakladığı için; bu metotlar tüm cüzdanları baştan sona tarayıp hedef adresi eşleşenleri filtreler. Algoritmik zaman karmaşıklığı O(V + E) seviyesindedir.*/
        decimal total = 0m;

        foreach (var walletAddress in _addresses)
        {
            foreach (var transaction in BatuhanGetOutgoingTransactions(walletAddress))
            {
                if (transaction.ToAddress == address)
                {
                    total += transaction.Amount;
                }
            }
        }

        return total;
    }

    public decimal BatuhanGetOutgoingTotal(string address)
    {
        decimal total = 0m;

        foreach (var transaction in BatuhanGetOutgoingTransactions(address))
        {
            total += transaction.Amount;
        }

        return total;
    }

    public List<string> BatuhanBreadthFirstTraversal(string startAddress)
    {
    /*BatuhanBreadthFirstTraversal: Ağ üzerinde Genişlik Öncelikli Arama (BFS) dolaşımı yapar. 
    Projeye özgü UmmetQueue yapısını kullanarak komşuları işler ve ziyaret edilen cüzdan adreslerini tekrar işleme sokmamak için bir HashTable içerisinde O(1) hızında işaretler.*/
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_wallets.ContainsKey(startAddress))
        {
            return order;
        }

        visited.Add(startAddress, true);
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var current = queue.Dequeue();
            order.Add(current);

            foreach (var transaction in BatuhanGetOutgoingTransactions(current))
            {
                if (!visited.ContainsKey(transaction.ToAddress))
                {
                    visited.Add(transaction.ToAddress, true);
                    queue.Enqueue(transaction.ToAddress);
                }
            }
        }

        return order;
    }

    public List<string> BatuhanDepthFirstTraversal(string startAddress)
    {
    /*BatuhanDepthFirstTraversal: Ağ üzerinde Derinlik Öncelikli Arama (DFS) dolaşımı yapar. 
    UmmetStack yapısını kullanır. Standart DFS iterasyon sırasını koruyabilmek amacıyla, çıkan transfer kenarlarını sondan başa doğru (ters döngü ile) yığına iter.*/
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var stack = new UmmetStack<string>();

        if (!_wallets.ContainsKey(startAddress))
        {
            return order;
        }

        stack.Push(startAddress);

        while (!stack.IsEmpty)
        {
            var current = stack.Pop();

            if (visited.ContainsKey(current))
            {
                continue;
            }

            visited.Add(current, true);
            order.Add(current);

            var outgoing = BatuhanGetOutgoingTransactions(current);
            for (int i = outgoing.Count - 1; i >= 0; i--)
            {
                var nextAddress = outgoing[i].ToAddress;
                if (!visited.ContainsKey(nextAddress))
                {
                    stack.Push(nextAddress);
                }
            }
        }

        return order;
    }

    private void BatuhanAddVertexIfMissing(string address)
    {
        if (!_wallets.ContainsKey(address))
        {
            BatuhanAddVertex(new WalletNode(address));
        }
    }

    // 1. Geriye Dönük Akış İçin Gelen Kenarları Bulma Metodu
    public IReadOnlyList<TransactionEdge> BatuhanGetIncomingEdges(string address)
    {
        var incomingEdges = new List<TransactionEdge>();
        foreach (var walletAddress in _addresses)
        {
            foreach (var edge in BatuhanGetOutgoingTransactions(walletAddress))
            {
                if (edge.ToAddress == address)
                {
                    incomingEdges.Add(edge);
                }
            }
        }
        return incomingEdges;
    }

    /*BatuhanGetForwardFundFlow / BatuhanGetBackwardFundFlow: 
    İleriye ve geriye dönük fon izleme (iz sürme) algoritmalarıdır. 
    Temel ağ taramalarından (BFS/DFS) temel bir farkı vardır: Bu metotlar sonsuz döngüden korunmak için cüzdan düğümlerini değil, benzersiz işlem kimliklerini (TransactionId) ziyaret edildi olarak işaretlerler.
    Bu mühendislik yaklaşımı, blokzincirdeki kapalı devre para aklama döngülerinin (A -> B -> C -> A) yazılımı çökertmesini önler.*/


    public List<TransactionEdge> BatuhanGetForwardFundFlow(string startAddress)
    {
    /*Kullanım Amacı: Belirli bir cüzdandan (örneğin şüpheli bir kaynaktan) çıkan paranın, ağ üzerindeki hangi cüzdanlara, hangi miktarlarla ve hangi yollardan dağıldığını adım adım takip etmektir.*/
    /*Çalışma Prensibi: Algoritma, özel jenerik kuyruk yapısını (UmmetQueue) kullanarak Genişlik Öncelikli Arama (BFS) mantığıyla çalışır. Başlangıç düğümünden çıkan tüm işlemleri kuyruğa alır ve ağaç yapısında aşağıya doğru ilerler.*/
    /*Döngü Koruması (Kritik Mühendislik Farkı): Klasik BFS algoritmaları ziyaret edilen 'düğümleri' (cüzdanları) işaretlerken, bu metot FNV-1a destekli karma tablosu (visitedEdges) üzerinde işlemlerin benzersiz kimliklerini (TransactionId) işaretler.
    Eğer A cüzdanından B'ye, B'den C'ye ve C'den tekrar A'ya para gönderilmişse (kapalı devre transfer), düğüm tabanlı bir arama sonsuz döngüye girip belleği taşırabilir (StackOverflow).
    Ancak işlem ID'si tutulduğunda, sistem aynı transferi ikinci kez işlemez ve algoritma güvenle sonlanır.*/
        var flowEdges = new List<TransactionEdge>();
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_wallets.ContainsKey(startAddress)) return flowEdges;
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var currentAddress = queue.Dequeue();
            foreach (var transaction in BatuhanGetOutgoingTransactions(currentAddress))
            {
                if (!visitedEdges.ContainsKey(transaction.TransactionId))
                {
                    visitedEdges.Add(transaction.TransactionId, true);
                    flowEdges.Add(transaction);
                    queue.Enqueue(transaction.ToAddress);
                }
            }
        }
        return flowEdges;
    }

    public List<TransactionEdge> BatuhanGetBackwardFundFlow(string startAddress)
    {
    /*Kullanım Amacı: Belirli bir hedef cüzdana (örneğin bir kripto para borsasına) ulaşan paranın kök kaynağını, yani ağa ilk nereden girdiğini geriye doğru izleyerek tespit etmektir. Kara para aklama (AML) tespitlerinde kullanılan temel yaklaşımdır.*/
    /*Tersine Dolaşım (Reverse Traversal): Grafın komşuluk listesi mimarisi yapısal olarak sadece 'giden' (outgoing) işlemleri O(1) hızında bulmaya elverişlidir. Geriye dönük iz sürmek için algoritma, akıntının tersine kürek çekmek zorundadır.*/
    /*Çalışma Prensibi: BFS tabanlı bu algoritma, kuyruktan çıkardığı her cüzdan için öncelikle BatuhanGetIncomingEdges metodunu çağırır. Bu metot, tüm grafı tarayarak o anki cüzdana 'gelen' işlemleri bulur. 
    Ardından, algoritma bu gelen işlemlerin kaynak adreslerini (FromAddress) kuyruğa ekleyerek hedeften kaynağa doğru adım adım geri gider. Döngü koruması (TransactionId takibi) bu metotta da aktif olarak kullanılarak analizin kilitlenmesi engellenmiştir.*/
        var flowEdges = new List<TransactionEdge>();
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_wallets.ContainsKey(startAddress)) return flowEdges;
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var currentAddress = queue.Dequeue();
            foreach (var edge in BatuhanGetIncomingEdges(currentAddress))
            {
                if (!visitedEdges.ContainsKey(edge.TransactionId))
                {
                    visitedEdges.Add(edge.TransactionId, true);
                    flowEdges.Add(edge);
                    queue.Enqueue(edge.FromAddress);
                }
            }
        }
        return flowEdges;
    }
    
    public IReadOnlyList<TransactionEdge> BatuhanGetOutgoingEdges(string address)
    {
        return BatuhanGetOutgoingTransactions(address);
    }
}
