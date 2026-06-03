using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainAnalysis.Models;
using BlockchainAnalysis.DataStructures;
using BlockchainAnalysis.Core;

namespace BlockchainAnalysis.App
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Console.WriteLine("============================================================");
            Console.WriteLine(" BLOKZINCIR ISLEM AGLARI ANALIZI - FAZ 1 DEMO");
            Console.WriteLine("============================================================");
            Console.WriteLine();

            // 1. Graf Motorunun ve Mock Verilerin Kurulumu
            BlockchainGraph graph = new BlockchainGraph();
            var transactions = GetMockTransactions();
            var TransactionEdges = new List<TransactionEdge>();

            Console.WriteLine("1) Sentetik Islem Verileri");
            Console.WriteLine("--------------------------");

            foreach (var tx in transactions)
            {
                var fromNode = new WalletNode(tx.From);
                var toNode = new WalletNode(tx.To);

                var edge = new TransactionEdge(fromNode, toNode, tx.Amount, tx.Fee);
                graph.BatuhanAddEdge(edge);
                TransactionEdges.Add(edge); // Merkle ve Hash işlemleri için listeye alıyoruz

                string shortId = edge.TransactionId.Substring(0, 8);
                Console.WriteLine($"{shortId} | {tx.From} -> {tx.To} | Amount: {tx.Amount:F2} | Fee: {tx.Fee:F2} | Time: {edge.Timestamp.ToString("HH:mm:ss")}");
            }
            Console.WriteLine();

            // 2. Hash Table İstatistikleri (Gerçek HashTable Sınıfından)
            Console.WriteLine("2) Hash Table - Cuzdan ve Islem ID Erisimi");
            Console.WriteLine("------------------------------------------");

            // Senin yazdığın FNV-1a Hash fonksiyonunu kullanan tablolar oluşturuluyor
            var walletTable = new HashTable<string, string>(16, WalletHashFunctions.HashFNV1a);
            var txTable = new HashTable<string, TransactionEdge>(16, WalletHashFunctions.HashFNV1a);

            // Verileri tablolara işliyoruz
            foreach (var edge in TransactionEdges)
            {
                if (!walletTable.ContainsKey(edge.FromAddress)) walletTable.Add(edge.FromAddress, "Wallet");
                if (!walletTable.ContainsKey(edge.ToAddress)) walletTable.Add(edge.ToAddress, "Wallet");

                txTable.Add(edge.TransactionId, edge);
            }

            // İstatistikleri doğrudan senin sınıfından (GetStats) çekiyoruz
            var wStats = walletTable.GetStats();
            var tStats = txTable.GetStats();

            Console.WriteLine($"Cuzdan sayisi: {wStats.Count}, Bucket: {wStats.BucketCount}, Load: {wStats.LoadFactor:F2}, Collision: {wStats.TotalCollisions}");
            Console.WriteLine($"Islem sayisi: {tStats.Count}, Bucket: {tStats.BucketCount}, Load: {tStats.LoadFactor:F2}, Collision: {tStats.TotalCollisions}");

            Console.WriteLine("O(1) cuzdan erisimi: " + (walletTable.ContainsKey("0xA1B2") ? "0xA1B2" : "Bulunamadi"));

            var sampleTx = TransactionEdges.FirstOrDefault(t => t.FromAddress == "0xC3D4");
            if (sampleTx != null && txTable.TryGetValue(sampleTx.TransactionId, out var foundTx))
            {
                string sampleShortId = foundTx.TransactionId.Substring(0, 8);
                Console.WriteLine($"O(1) islem erisimi: {sampleShortId} -> {foundTx.FromAddress} -> {foundTx.ToAddress}");
            }
            Console.WriteLine();

            // 3. Yönlü Graf Çıktısı 
            Console.WriteLine("3) Yonlu Graf - Transfer Agi");
            Console.WriteLine("----------------------------");

            string[] orderedAddresses = { "0xA1B2", "0xC3D4", "0xE5F6", "0x7788" };
            foreach (var address in orderedAddresses)
            {
                var node = graph.BatuhanGetNode(address);
                var edges = graph.BatuhanGetOutgoingEdges(address);

                Console.WriteLine($"{node.Address} | Balance: {node.Balance:F2}");
                foreach (var edge in edges)
                {
                    Console.WriteLine($"  -> {edge.ToAddress} | Amount: {edge.Amount:F2} | Time: {edge.Timestamp.ToString("HH:mm:ss")}");
                }
            }
            Console.WriteLine();

            // 4. Graf Dolaşımı 
            Console.WriteLine("4) Queue ve Stack - Graf Dolasimi");
            Console.WriteLine("---------------------------------");
            string startNode = "0xA1B2";

            var bfsResult = graph.BatuhanBreadthFirstTraversal(startNode);
            Console.WriteLine($"BFS (Queue) from {startNode}: " + string.Join(" -> ", bfsResult));

            var dfsResult = graph.BatuhanDepthFirstTraversal(startNode);
            Console.WriteLine($"DFS (Stack) from {startNode}: " + string.Join(" -> ", dfsResult));
            Console.WriteLine();

            // 5. Merkle Tree Testi (Gerçek MerkleTree Sınıfından)
            Console.WriteLine("5) Merkle Tree - Islem Butunlugu");
            Console.WriteLine("--------------------------------");

            // İşlemleri benzersiz birer "payload" (yük) stringine çeviriyoruz
            var payloads = new List<string>();
            for (int i = 0; i < TransactionEdges.Count; i++)
            {
                var edge = TransactionEdges[i];
                string payload = $"{edge.TransactionId}{edge.FromAddress}{edge.ToAddress}{edge.Amount}";
                payloads.Add(payload);

                // Kendi yazdığın ComputeHash metodu ile SHA256 hesabı
                string hash = AliMerkleTree.ComputeHash(payload);
                Console.WriteLine($"Tx Hash {i + 1}: {hash.Substring(0, 16)}...");
            }

            // Ağacı inşa et ve kökü al
            var merkleTree = new AliMerkleTree();
            string rootHash = merkleTree.Build(payloads);
            Console.WriteLine($"Merkle Root: {rootHash.Substring(0, 24)}...");

            // Kendi Verify metotların ile verileri doğrulama
            bool isValid = merkleTree.Verify(payloads, rootHash);
            Console.WriteLine($"Dogrulama sonucu: {isValid}");

            // Verinin manipüle edilmesi ve reddedilmesi simülasyonu
            var tamperedPayloads = new List<string>(payloads);
            tamperedPayloads[0] = "SAHTE_ISLEM_VERISI_12345";
            bool isTamperedValid = merkleTree.Verify(tamperedPayloads, rootHash);
            Console.WriteLine($"Degistirilmis veri dogrulama sonucu: {isTamperedValid}");
            Console.WriteLine();

            // 6. Sonuç ve Özet
            Console.WriteLine("6) Faz 1 Kontrol Ozeti");
            Console.WriteLine("----------------------");
            Console.WriteLine("[OK] Directed Graph: Cuzdanlar dugum, transferler yonlu kenar olarak modellendi.");
            Console.WriteLine("[OK] Merkle Tree: Islem hashleri ile Merkle Root uretildi ve dogrulandi.");
            Console.WriteLine("[OK] Hash Table: Cuzdan adresleri ve islem ID'leri O(1) ortalama erisim icin tutuldu.");
            Console.WriteLine("[OK] Queue: BFS dolasimi icin kullanildi.");
            Console.WriteLine("[OK] Stack: DFS dolasimi icin kullanildi.");

            Console.WriteLine("\nCikmak icin Enter'a basin...");
            Console.ReadLine();  
            */
            TestFundFlowFilters();
        }

        public static void TestFundFlowFilters()
        {
            Console.WriteLine("============================================================");
            Console.WriteLine(" FAZ 2 DEMO - FUND FLOW TRACKER FILTRELEME TESTI");
            Console.WriteLine("============================================================");

            var graph = new BlockchainGraph();

            // 1. Test Düğümlerini (Cüzdanları) Oluşturalım
            var w1 = new WalletNode("0xALICE");
            var w2 = new WalletNode("0xBOB");
            var w3 = new WalletNode("0xCHARLIE");
            var w4 = new WalletNode("0xDAVID");

            graph.BatuhanAddVertex(w1);
            graph.BatuhanAddVertex(w2);
            graph.BatuhanAddVertex(w3);
            graph.BatuhanAddVertex(w4);

            // 2. İşlemleri Ekleyelim (Zaman damgaları ardışık olsun diye kısa beklemeler koyuyoruz)
            graph.BatuhanAddEdge(new TransactionEdge(w1, w2, 50m, 1m)); // Alice -> Bob (50)
            System.Threading.Thread.Sleep(50);

            graph.BatuhanAddEdge(new TransactionEdge(w2, w3, 150m, 2m)); // Bob -> Charlie (150)
            System.Threading.Thread.Sleep(50);

            graph.BatuhanAddEdge(new TransactionEdge(w1, w4, 20m, 0.5m)); // Alice -> David (20)
            System.Threading.Thread.Sleep(50);

            graph.BatuhanAddEdge(new TransactionEdge(w3, w4, 200m, 5m)); // Charlie -> David (200)

            // 3. Sizin Yazdığınız Tracker'ı Başlatalım
            var tracker = new FundFlowTracker(graph);

            // --- TEST SENARYOLARI ---

            Console.WriteLine("\nSenaryo A: 0xALICE'ten cikan TUM fon akisi (Ileriye Donuk BFS)");
            var allFlow = tracker.BatuhanTrackForwardFlow("0xALICE");
            foreach (var tx in allFlow)
            {
                Console.WriteLine($" -> {tx.FromAddress} gonderdi {tx.ToAddress} | Miktar: {tx.Amount} | Zaman: {tx.Timestamp:HH:mm:ss.fff}");
            }

            Console.WriteLine("\nSenaryo B: 0xALICE'ten cikan akis (Miktar Filtresi: Sadece 100 birim uzeri)");
            var highValueFlow = tracker.BatuhanTrackForwardFlow("0xALICE", minAmount: 100m);
            foreach (var tx in highValueFlow)
            {
                Console.WriteLine($" -> {tx.FromAddress} gonderdi {tx.ToAddress} | Miktar: {tx.Amount} | Zaman: {tx.Timestamp:HH:mm:ss.fff}");
            }

            Console.WriteLine("\nSenaryo C: 0xDAVID'e gelen fonlarin asil kaynagi (Geriye Donuk BFS)");
            var pastFlow = tracker.BatuhanTrackBackwardFlow("0xDAVID");
            foreach (var tx in pastFlow)
            {
                Console.WriteLine($" <- {tx.FromAddress} gonderdi {tx.ToAddress} | Miktar: {tx.Amount} | Zaman: {tx.Timestamp:HH:mm:ss.fff}");
            }

            Console.WriteLine("\nTest tamamlandi. Cikmak icin Enter'a basin...");
            Console.ReadLine();
        }

        private static List<(string From, string To, decimal Amount, decimal Fee)> GetMockTransactions()
        {
            return new List<(string, string, decimal, decimal)>
            {
                ("0xA1B2", "0xC3D4", 12.50m, 0.10m),
                ("0xA1B2", "0xE5F6", 4.25m, 0.03m),
                ("0xC3D4", "0x7788", 2.00m, 0.02m),
                ("0xE5F6", "0x7788", 1.75m, 0.01m),
                ("0x7788", "0xA1B2", 0.50m, 0.01m)
            };
        } 
 
        
    }
}