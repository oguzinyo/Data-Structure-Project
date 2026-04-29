using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

/* Sınıfın amacı: Düğümleri (WalletNode) ve kenarları (TransactionEdge) bir araya getiren
 * anlamlı bir graf modelisini oluşturur. Tüm hesapları ve hesaplar arası fon akışını tek bir
 * çatı altında toplar.
 */

namespace BlockchainAnalysis.Models
{
    public class BlockchainGraph
    {
        // Tüm cüzdanları O(1) sürede bulabilmek ve aynı graf üzerine
        // veri yazılmaya çalışılırken veri çakışmalarını önlemek için
        // Thread-Safe Hash Table (Sözlük) kullanıyoruz.
        public ConcurrentDictionary<string, WalletNode> Wallets { get; private set; }

        // Komşuluk Listesi (Adjacency List) - Thread-Safe
        // Her bir cüzdan adresine karşılık, o cüzdanın yaptığı transferlerin bir listesini tutar.
        // Adjacency matrix yerine adjacency list kullanırız çünkü blokzincir ağları seyrek graf (sparse graf)
        // yapısına sahiplerdir. milyonlarca cüzdan olsa dahi bi cüzdan sadece birkaç kişiyle işlem yapar.
        // böylece bellek tasarrufu yapmış oluruz.
        public ConcurrentDictionary<string, ConcurrentBag<TransactionEdge>> AdjacencyList { get; private set; }

        public BlockchainGraph() //constructor
        {
            Wallets = new ConcurrentDictionary<string, WalletNode>();
            AdjacencyList = new ConcurrentDictionary<string, ConcurrentBag<TransactionEdge>>();
        }

        // Graf yapısına yeni bir düğüm (cüzdan) ekler
        public void AddWallet(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return; //gelen adres boş mu değil mi

            if (!Wallets.ContainsKey(address)) //cüzdan halihazırda sistemde var mı
            {
                var newNode = new WalletNode(address);
                Wallets.TryAdd(address, newNode);
                AdjacencyList.TryAdd(address, new ConcurrentBag<TransactionEdge>());
            }
        }

        // Graf yapısına yönlü bir kenar (transfer işlemi) ekler
        public void AddTransaction(string fromAddress, string toAddress, double amount)
        {
            // Gönderen veya alıcı sistemde yoksa, otomatik olarak oluştur ve ağa dahil et
            if (!Wallets.ContainsKey(fromAddress)) AddWallet(fromAddress);
            if (!Wallets.ContainsKey(toAddress)) AddWallet(toAddress);

            var fromNode = Wallets[fromAddress];
            var toNode = Wallets[toAddress];

            // Yeni yönlü kenarı (işlemi) oluştur
            var transaction = new TransactionEdge(fromNode, toNode, amount);

            // İşlemi gönderenin komşuluk listesine ekle (Yönlü graf olduğu için sadece gönderene eklenir)
            AdjacencyList[fromAddress].Add(transaction);

            // Bakiye (Balance) Güncellemesi - Sadeleştirilmiş model
            fromNode.Balance -= amount; //gönderenden azaltır
            toNode.Balance += amount; //alıcının arttırır
        }
    }
}