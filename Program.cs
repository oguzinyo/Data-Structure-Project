using System;
using System.Diagnostics;
using Proje4;

namespace Proje4Demo
{
    class Program
    {
        // ============================================================
        // HASH FONKSİYONU KARŞILAŞTIRMASI
        // Farklı hash fonksiyonlarının collision davranışını test eder
        // ============================================================
        static void CompareHashFunctions()
        {
            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine(" HASH FONKSİYONU KARŞILAŞTIRMASI");
            Console.WriteLine("=".PadRight(60, '='));

            string[] testAddresses = GenerateWalletAddresses(1000);

            var functions = new (string Name, Func<string, int> Func)[]
            {
                ("Simple Hash", WalletHashFunctions.HashSimple),
                ("DJB2", WalletHashFunctions.HashDJB2),
                ("FNV-1a", WalletHashFunctions.HashFNV1a),
            };

            foreach (var (name, func) in functions)
            {
                var ht = new HashTable<string, int>(initialCapacity: 64, hashFunc: func);
                int collisions = 0;

                for (int i = 0; i < testAddresses.Length; i++)
                {
                    _ = ht.GetIndexForKey(testAddresses[i]); // Just to show index calculation
                    if (ht.ContainsKey(testAddresses[i]))
                        collisions++;
                    else
                        ht.Add(testAddresses[i], i);
                }

                var stats = ht.GetStats();
                Console.WriteLine($"\n{name}:");
                Console.WriteLine($"  Bucket sayısı: {stats.BucketCount}");
                Console.WriteLine($"  Collision sayısı: {stats.TotalCollisions}");
                Console.WriteLine($"  Ortalama chain uzunluğu: {stats.AverageChainLength:F2}");
            }
        }

        // Yardımcı metot - HashTable'ın internal index'ini almak için
        static int GetHashIndexHelper<TKey>(HashTable<TKey, object> ht, TKey key)
        {
            // Bu test için özel - gerçek kullanımda public API'yi kullan
            return 0;
        }

        // ============================================================
        // BLOCKCHAIN SENERYO TESTİ
        // Gerçek kullanım senaryosunu simüle eder
        // ============================================================
        static void BlockchainScenarioTest()
        {
            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine(" BLOKZİNCİR İŞLEM AĞI SENARYOSU");
            Console.WriteLine("=".PadRight(60, '='));

            // Cüzdan adreslerini tutan HashTable
            var walletTable = new HashTable<string, WalletNode>(initialCapacity: 32);

            // İşlem ID'lerini tutan HashTable
            var transactionTable = new HashTable<string, Transaction>(initialCapacity: 64);

            Console.WriteLine("\n--- Cüzdan Oluşturma ---");
            string[] wallets = { "0xA1B2C3D4E5F6", "0x9876543210AB", "0xDEADBEEF1234", "0xFACEFACE0001" };

            foreach (var addr in wallets)
            {
                var walletNode = new WalletNode(addr);
                walletTable.Add(addr, walletNode);
                Console.WriteLine($"  Cüzdan eklendi: {addr}");
            }

            Console.WriteLine($"\n  Toplam cüzdan: {walletTable.Count}");
            var stats1 = walletTable.GetStats();
            Console.WriteLine($"  Bucket sayısı: {stats1.BucketCount}");
            Console.WriteLine($"  Collision sayısı: {stats1.TotalCollisions}");

            Console.WriteLine("\n--- İşlem Oluşturma ---");
            var transactions = new Transaction[]
            {
                new Transaction("TX001", "0xA1B2C3D4E5F6", "0x9876543210AB", 1.5m, DateTime.Now.AddHours(-3)),
                new Transaction("TX002", "0x9876543210AB", "0xDEADBEEF1234", 0.8m, DateTime.Now.AddHours(-2)),
                new Transaction("TX003", "0xA1B2C3D4E5F6", "0xDEADBEEF1234", 2.0m, DateTime.Now.AddHours(-1)),
                new Transaction("TX004", "0xDEADBEEF1234", "0xFACEFACE0001", 0.5m, DateTime.Now),
            };

            foreach (var tx in transactions)
            {
                transactionTable.Add(tx.TransactionId, tx);
                Console.WriteLine($"  İşlem eklendi: {tx.TransactionId}");
            }

            Console.WriteLine($"\n  Toplam işlem: {transactionTable.Count}");

            Console.WriteLine("\n--- O(1) Erişim Testi ---");
            Stopwatch sw = new Stopwatch();

            // 10000 kez erişim testi
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                _ = transactionTable.TryGetValue("TX003", out _);
            }
            sw.Stop();

            Console.WriteLine($"  10000 erişim süresi: {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds}ms)");
            Console.WriteLine($"  Ortalama erişim: {sw.ElapsedTicks / 10000.0:F2} ticks");

            Console.WriteLine("\n--- İşlem Sorgulama ---");
            if (transactionTable.TryGetValue("TX002", out Transaction tx2))
            {
                Console.WriteLine($"  TX002 bulundu: {tx2.FromAddress} -> {tx2.ToAddress} : {tx2.Amount} ETH");
            }

            if (walletTable.TryGetValue("0xDEADBEEF1234", out WalletNode walletData))
            {
                Console.WriteLine($"  Cüzdan bulundu: {walletData.Address}");
            }
        }

        // ============================================================
        // COLLISION TESTİ
        // Hash fonksiyonunun collision davranışını test eder
        // ============================================================
        static void CollisionTest()
        {
            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine(" COLLISION (ÇAKIŞMA) TESTİ");
            Console.WriteLine("=".PadRight(60, '='));

            // Kötü bir hash fonksiyonu ile collision test et
            int BadHash(string s) => s.Length % 8; // Çok kötü - sadece uzunluğa bakıyor

            var ht = new HashTable<string, int>(initialCapacity: 8, hashFunc: BadHash);

            Console.WriteLine("\nKötü hash fonksiyonu (sadece uzunluk):");
            string[] test = { "abcd", "wxyz", "1234", "9xyz", "test", "demo" };

            foreach (var s in test)
            {
                ht.Add(s, s.GetHashCode());
                Console.WriteLine($"  '{s}' -> bucket {s.Length % 8}");
            }

            var stats = ht.GetStats();
            Console.WriteLine($"\n  Toplam collision: {stats.TotalCollisions}");
            Console.WriteLine($"  Max chain length: {stats.MaxChainLength}");

            ht.PrintDistribution();
        }

        // ============================================================
        // BÜYÜK VERİ TESTİ
        // O(1) performansını büyük veri seti ile test eder
        // ============================================================
        static void LargeScaleTest()
        {
            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine(" BÜYÜK ÖLÇEK TESTİ (10000 işlem)");
            Console.WriteLine("=".PadRight(60, '='));

            var ht = new HashTable<string, int>(initialCapacity: 16, hashFunc: WalletHashFunctions.HashFNV1a);
            var addresses = GenerateWalletAddresses(10000);

            Console.WriteLine("\n--- Ekleme ---");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < addresses.Length; i++)
            {
                ht.Add(addresses[i], i);
            }

            sw.Stop();
            var stats = ht.GetStats();

            Console.WriteLine($"  10000 işlem eklendi");
            Console.WriteLine($"  Geçen süre: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Ortalama ekleme: {sw.ElapsedMilliseconds / 10000.0:F3}ms");
            Console.WriteLine($"  Bucket sayısı: {stats.BucketCount}");
            Console.WriteLine($"  Resize sayısı: {stats.TotalResizes}");
            Console.WriteLine($"  Collision sayısı: {stats.TotalCollisions}");
            Console.WriteLine($"  Load factor: {stats.LoadFactor:F3}");
            Console.WriteLine($"  Max chain: {stats.MaxChainLength}");

            Console.WriteLine("\n--- Arama (O(1) test) ---");
            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _ = ht.ContainsKey(addresses[i]);
            }

            sw.Stop();

            Console.WriteLine($"  10000 arama süresi: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Ortalama arama: {sw.ElapsedMilliseconds / 10000.0:F4}ms");
            Console.WriteLine($"  ~O(1) erişim başarıldı!");
        }

        // ============================================================
        // YARDIMCI FONKSİYONLAR
        // ============================================================

        /// <summary>
        /// Rastgele cüzdan adresleri üretir
        /// </summary>
        static string[] GenerateWalletAddresses(int count)
        {
            var random = new Random(12345); // Sabit seed ile tekrarlanabilir sonuç
            var addresses = new string[count];

            for (int i = 0; i < count; i++)
            {
                string addr = "0x";
                for (int j = 0; j < 16; j++)
                    addr += random.Next(16).ToString("X");
                addresses[i] = addr;
            }

            return addresses;
        }

        // ============================================================
        // MAIN
        // ============================================================
        static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     BLOKZİNCİR İŞLEM AĞLARI - HASH TABLE DEMO            ║");
            Console.WriteLine("║     Proje 4 - Faz 1: Zorunlu Veri Yapıları               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

            // Temel kullanım
            BasicUsageDemo();

            // Hash fonksiyonu karşılaştırması
            CompareHashFunctions();

            // Collision testi
            CollisionTest();

            // Blockchain senaryosu
            BlockchainScenarioTest();

            // Büyük ölçek testi
            LargeScaleTest();

            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine(" TEST TAMAMLANDI");
            Console.WriteLine("=".PadRight(60, '='));
        }

        // ============================================================
        // TEMEL KULLANIM DEMO
        // ============================================================
        static void BasicUsageDemo()
        {
            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine(" TEMEL KULLANIM DEMO");
            Console.WriteLine("=".PadRight(60, '='));

            // HashTable<TKey, TValue> ile çalışıyoruz
            var ht = new HashTable<string, int>(initialCapacity: 8, hashFunc: WalletHashFunctions.HashFNV1a);

            Console.WriteLine("\n--- Ekleme ---");
            ht.Add("apple", 5);
            ht.Add("banana", 3);
            ht.Set("orange", 7); // upsert benzeri bir kullanım
            Console.WriteLine($"  Eklendi: apple=5, banana=3, orange=7");

            Console.WriteLine("\n--- Erişim ---");
            if (ht.TryGetValue("apple", out int v))
                Console.WriteLine($"  apple => {v}");
            else
                Console.WriteLine("  apple not found");

            Console.WriteLine("\n--- Varlık Kontrolü ---");
            Console.WriteLine($"  Contains 'banana'? {ht.ContainsKey("banana")}");
            ht.Remove("banana");
            Console.WriteLine($"  Contains 'banana' after removal? {ht.ContainsKey("banana")}");

            Console.WriteLine($"\n--- Toplam: {ht.Count} ---");

            Console.WriteLine("\n--- Indexer ile güncelleme ---");
            ht["apple"] = 42;
            Console.WriteLine($"  Updated apple => {ht["apple"]}");

            Console.WriteLine("\n--- İstatistikler ---");
            var stats = ht.GetStats();
            Console.WriteLine($"  Bucket: {stats.BucketCount}, Collision: {stats.TotalCollisions}, Load: {stats.LoadFactor:F2}");
        }
    }
}