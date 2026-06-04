using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures;

public class BlockchainGraph : IGraph
/*Bu sınıf, blokzincir işlem ağını temsil eden, çoklu iş parçacığı güvenliğine (thread-safe) sahip ve gelişmiş rota bulma algoritmaları barındıran operasyonel graf motorudur. Temel IGraph arayüzünü uygular ve sistemin ana analitik katmanına veri sağlar.*/

/*
    BLOCKCHAINGRAPH İLE DIRECTEDGRAPH ARASINDAKİ FARK NEDİR?

    Her iki sınıf da IGraph sözleşmesine uysa ve komşuluk listesi mimarisini kullansa da, tasarım amaçları ve mühendislik yaklaşımları açısından üç temel farkla ayrılırlar:

    1. Kilitleme Stratejisi (Locking Strategy)
    
    - DirectedGraph: Mikro düzeyde (Fine-Grained) kilitleme yapar. Graf yapısına yeni bir kenar eklendiğinde grafı kilitlemez, 
    sadece işlemi gerçekleştiren ilgili iki cüzdanın bakiye kilitlerini (BalanceLock) kullanır. Bu durum yapısal değişikliklerde yarış durumlarına açıktır ancak sadece bakiye güncellemeleri için yüksek hız sağlar.
    
    - BlockchainGraph: Makro düzeyde (Coarse-Grained) kilitleme yapar. _graphLock nesnesi ile düğüm ve kenar ekleme işlemlerini tamamen senkronize eder. 
    Gerçek zamanlı ve çoklu simülasyon ortamlarında (örneğin API üzerinden aynı anda yüzlerce istek geldiğinde) veri bütünlüğünü kesin olarak garanti altına alır.
    
    2. Kapsülleme (Encapsulation) ve Bakiye Yönetimi
    
    - DirectedGraph: Bakiye güncellemesini graf sınıfının kendi içinde manuel olarak yapar (wallet.Balance -= amount). Nesne yönelimli tasarımın kapsülleme kuralını kısmen esnetir.
    
    - BlockchainGraph: Bakiye güncelleme sorumluluğunu tamamen veri modeline devreder. WalletNode sınıfı içinde önceden tanımlanmış, kendi kilidine sahip olan DeductFunds ve AddFunds metotlarını çağırır. 
    Bu yaklaşım İlgilerin Ayrılığı (Separation of Concerns) prensibine tam uygundur.
    
    3. Hedef ve Kapsam
    
    - DirectedGraph: Graf dolaşım hızını ölçmek, bellek yapılarını test etmek ve kilit maliyetlerinden kaçınmak için tasarlanmış yalın bir temel veri yapısıdır.
    
    - BlockchainGraph: Projenin Faz 2 ve Faz 3 gereksinimlerinde istenen "Maksimum kapasite yolu" veya "Hedef düğümlere ulaşım analizi" gibi özel operasyonları içeren, canlı sisteme (API ve Frontend'e) bağlanmaya hazır, tam donanımlı analiz motorudur.


*/

{
    // Eski GraphNode yerine doğrudan WalletNode kullanıyoruz
    private readonly HashTable<string, WalletNode> _nodes = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly HashTable<string, List<TransactionEdge>> _adjacencyList = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly List<string> _addresses = new();
    private readonly object _graphLock = new object();

    public void BatuhanAddVertex(WalletNode wallet)
    {
        lock (_graphLock)
        {
            BatuhanAddNodeIfMissing(wallet.Address);
        }
    }

public void BatuhanAddEdge(TransactionEdge edge)
    {
        lock (_graphLock)
        {
            BatuhanAddNodeIfMissing(edge.FromAddress);
            BatuhanAddNodeIfMissing(edge.ToAddress);

            _adjacencyList[edge.FromAddress].Add(edge);
        }

        // Kendi yazdığın thread-safe metotlarla bakiyeleri güncelliyoruz
        _nodes[edge.FromAddress].DeductFunds(edge.Amount);
        _nodes[edge.ToAddress].AddFunds(edge.Amount);
    }

    public IReadOnlyList<string> BatuhanGetAddresses() => _addresses;

    public WalletNode BatuhanGetNode(string address)
    {
        if (_nodes.TryGetValue(address, out var node))
        {
            return node;
        }

        throw new KeyNotFoundException($"Graph node not found: {address}");
    }

public IReadOnlyList<TransactionEdge> BatuhanGetOutgoingEdges(string address)
    {
        lock (_graphLock)
        {
            if (!_adjacencyList.TryGetValue(address, out var edges))
            {
                return Array.Empty<TransactionEdge>();
            }
            
            // Okuma sırasında veri değişmesin diye listenin kopyasını döndürüyoruz
            return new List<TransactionEdge>(edges);
        }
    }

    public List<string> BatuhanBreadthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return order;
        }

        visited.Add(startAddress, true);
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var current = queue.Dequeue();
            order.Add(current);

            foreach (var edge in BatuhanGetOutgoingEdges(current))
            {
                if (!visited.ContainsKey(edge.ToAddress))
                {
                    visited.Add(edge.ToAddress, true);
                    queue.Enqueue(edge.ToAddress);
                }
            }
        }

        return order;
    }

    public List<string> BatuhanDepthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var stack = new UmmetStack<string>();

        if (!_nodes.ContainsKey(startAddress))
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

            var outgoingEdges = BatuhanGetOutgoingEdges(current);
            for (int i = outgoingEdges.Count - 1; i >= 0; i--)
            {
                var nextAddress = outgoingEdges[i].ToAddress;
                if (!visited.ContainsKey(nextAddress))
                {
                    stack.Push(nextAddress);
                }
            }
        }

        return order;
    }

    private void BatuhanAddNodeIfMissing(string address)
    {
        if (_nodes.ContainsKey(address))
        {
            return;
        }

        _nodes.Add(address, new WalletNode(address));
        _adjacencyList.Add(address, new List<TransactionEdge>());
        _addresses.Add(address);
    }

    // 1. Geriye Dönük Akış İçin Gelen Kenarları Bulma Metodu
    public IReadOnlyList<TransactionEdge> BatuhanGetIncomingEdges(string address)
    {
        var incomingEdges = new List<TransactionEdge>();
        foreach (var walletAddress in _addresses)
        {
            foreach (var edge in BatuhanGetOutgoingEdges(walletAddress))
            {
                if (edge.ToAddress == address)
                {
                    incomingEdges.Add(edge);
                }
            }
        }
        return incomingEdges;
    }

    // 2. İleriye Dönük Fon Akışı (İşlem Döndüren ve Döngü Korumalı BFS)
    public List<TransactionEdge> BatuhanGetForwardFundFlow(string startAddress)
    {
        var flowEdges = new List<TransactionEdge>();
        // Blokzincirdeki döngüleri (A -> B -> A) kırmak için ID bazlı takip
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return flowEdges;
        }

        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var currentAddress = queue.Dequeue();

            foreach (var edge in BatuhanGetOutgoingEdges(currentAddress))
            {
                if (!visitedEdges.ContainsKey(edge.TransactionId))
                {
                    visitedEdges.Add(edge.TransactionId, true);
                    flowEdges.Add(edge);
                    queue.Enqueue(edge.ToAddress); // Paranın gittiği yeni adresi kuyruğa ekle
                }
            }
        }

        return flowEdges;
    }

    // 3. Geriye Dönük Fon Kaynağı İzleme (İşlem Döndüren BFS)
    public List<TransactionEdge> BatuhanGetBackwardFundFlow(string startAddress)
    {
        var flowEdges = new List<TransactionEdge>();
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return flowEdges;
        }

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
                    queue.Enqueue(edge.FromAddress); // Paranın geldiği kaynak adresi kuyruğa ekle
                }
            }
        }

        return flowEdges;
    }

    /*Standart BFS ve DFS dolaşımlarına (BatuhanBreadthFirstTraversal, BatuhanDepthFirstTraversal) ve döngü korumalı fon akış izleyicilerine (BatuhanGetForwardFundFlow, BatuhanGetBackwardFundFlow) ek olarak iki özel analitik algoritma barındırır:*/


    // 1. Target Node Analysis: Finds the path between start and target addresses using BFS.
    public List<string> MehmetFindPath(string startAddress, string targetAddress)
    {
    /*MehmetFindPath (Hedef Rota Analizi): Belirtilen başlangıç adresinden hedef adrese giden geçerli bir transfer yolu olup olmadığını 
    Genişlik Öncelikli Arama (BFS) ile araştırır. Hedefe ulaşıldığında, parentMap (ebeveyn haritası) kullanılarak rota geriye doğru örülür ve kronolojik bir adres listesi olarak döndürülür.*/
        var path = new List<string>();
        var parentMap = new HashTable<string, string>(hashFunc: WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_nodes.ContainsKey(startAddress) || !_nodes.ContainsKey(targetAddress))
            return path;

        parentMap.Add(startAddress, null);
        queue.Enqueue(startAddress);

        bool found = false;
        while (!queue.IsEmpty)
        {
            var current = queue.Dequeue();
            if (current == targetAddress)
            {
                found = true;
                break;
            }

            foreach (var edge in BatuhanGetOutgoingEdges(current))
            {
                if (!parentMap.ContainsKey(edge.ToAddress))
                {
                    parentMap.Add(edge.ToAddress, current);
                    queue.Enqueue(edge.ToAddress);
                }
            }
        }

        if (found)
        {
            var curr = targetAddress;
            while (curr != null)
            {
                path.Insert(0, curr);
                curr = parentMap[curr];
            }
        }

        return path;
    }

    // 2. Maximum Capacity Path: Finds the route with the highest transaction volume.
    public List<string> MehmetFindMaxCapacityPath(string startAddress, string targetAddress)
    {
    /*MehmetFindMaxCapacityPath (Maksimum Kapasite Yolu): Ağ üzerindeki en yoğun finansal akış damarını bulan, Dijkstra algoritmasının bir varyasyonudur. 
    Klasik Dijkstra en kısa (veya en düşük maliyetli) yolu ararken, bu algoritma darboğaz (bottleneck) kapasitesi en geniş olan yolu arar. 
    Her iterasyonda hedef düğüme giden alternatifler arasından, geçebilecek maksimum transfer miktarını (Math.Min(capacities[current], edge.Amount)) hesaplayarak en güvenilir ve yüksek hacimli finansal rotayı tespit eder.*/
        var parentMap = new HashTable<string, string>(hashFunc: WalletHashFunctions.HashFNV1a);
        var capacities = new HashTable<string, decimal>(hashFunc: WalletHashFunctions.HashFNV1a);
        var addresses = BatuhanGetAddresses();

        foreach (var addr in addresses)
        {
            capacities.Add(addr, decimal.MinValue);
        }

        capacities[startAddress] = decimal.MaxValue;
        parentMap.Add(startAddress, null);

        var unvisited = new List<string>(addresses);

        while (unvisited.Count > 0)
        {
            string current = null;
            decimal maxCap = decimal.MinValue;

            foreach (var node in unvisited)
            {
                if (capacities[node] > maxCap)
                {
                    maxCap = capacities[node];
                    current = node;
                }
            }

            if (current == null || current == targetAddress || maxCap == decimal.MinValue)
                break;

            unvisited.Remove(current);

            foreach (var edge in BatuhanGetOutgoingEdges(current))
            {
                decimal pathCapacity = Math.Min(capacities[current], edge.Amount);

                if (pathCapacity > capacities[edge.ToAddress])
                {
                    capacities[edge.ToAddress] = pathCapacity;
                    if (!parentMap.ContainsKey(edge.ToAddress))
                        parentMap.Add(edge.ToAddress, current);
                    else
                        parentMap[edge.ToAddress] = current;
                }
            }
        }

        var path = new List<string>();
        if (parentMap.ContainsKey(targetAddress))
        {
            var curr = targetAddress;
            while (curr != null)
            {
                path.Insert(0, curr);
                curr = parentMap[curr];
            }
        }
        return path;
    }
}
