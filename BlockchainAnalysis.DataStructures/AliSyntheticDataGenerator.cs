using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures
{
    /*
     * =========================================================
     * SYNTHETIC DATA GENERATOR (HASH TABLE ENTEGRELİ VERSİYON)
     * =========================================================
     *
     * Amaç:
     * - Konsola hiçbir şey yazdırmadan tamamen sessiz çalışmak.
     * - Sabit seed (42) ile her çalıştırmada aynı veriyi üretmek.
     * - Constructor ile otomatik başlatma — dışarıdan tek satırla kullanım.
     * - Tüm verileri bellekteki tablolarda hazır tutmak.
     * - YENİ: Cüzdan ve İşlem verilerini O(1) erişim için HashTable'a kopyalamak.
     *
     * =========================================================
     */

    public class AliSyntheticDataGenerator
    {
        // Sabit seed — her çalıştırmada aynı veriyi garanti eder
        private readonly Random _random = new Random(42);

        // Singleton — kaç kere new'lense de tek instance çalışır
        private static AliSyntheticDataGenerator? _sharedInstance;
        private static readonly object _lock = new object();

        // =====================================================
        // BELLEK İÇİ VERİ TABANI TABLOLARI (STANDART LİSTELER)
        // =====================================================
        private List<BatuhanWalletNode> _walletsTable = new List<BatuhanWalletNode>();
        private List<BatuhanTransactionEdge> _transactionsTable = new List<BatuhanTransactionEdge>();

        private List<BatuhanTransactionEdge> _chainFlowPart = new List<BatuhanTransactionEdge>();
        private List<BatuhanTransactionEdge> _exchangePart = new List<BatuhanTransactionEdge>();
        private List<BatuhanTransactionEdge> _cyclePart = new List<BatuhanTransactionEdge>();
        private List<BatuhanTransactionEdge> _randomPart = new List<BatuhanTransactionEdge>();

        // =====================================================
        // ENTEGRE EDİLEN ÖZEL HASH TABLE YAPILARI
        // =====================================================
        private HashTable<string, BatuhanWalletNode> _walletsHashTable;
        private HashTable<string, BatuhanTransactionEdge> _transactionsHashTable;

        // Guard clause — her iki tablo (ve Hash Table'lar) kontrol ediliyor
        public bool IsInitialized => _walletsTable.Count > 0 && 
                                     _transactionsTable.Count > 0 && 
                                     _walletsHashTable != null && 
                                     _transactionsHashTable != null;

        // Dışarıdan erişim (Read-Only)
        public List<BatuhanWalletNode> WalletsTable => _walletsTable;
        public List<BatuhanTransactionEdge> TransactionsTable => _transactionsTable;
        public List<BatuhanTransactionEdge> ChainFlowPart => _chainFlowPart;
        public List<BatuhanTransactionEdge> ExchangePart => _exchangePart;
        public List<BatuhanTransactionEdge> CyclePart => _cyclePart;
        public List<BatuhanTransactionEdge> RandomPart => _randomPart;

        // YENİ: Dışarıdan O(1) arama yapmak için açılan HashTable'lar
        public HashTable<string, BatuhanWalletNode> WalletsHashTable => _walletsHashTable;
        public HashTable<string, BatuhanTransactionEdge> TransactionsHashTable => _transactionsHashTable;

        // =====================================================
        // CONSTRUCTOR — Otomatik başlatma
        // =====================================================
        public AliSyntheticDataGenerator(int hedefCuzdanSayisi = 20)
        {
            // Cüzdan adresleri için kendi yazdığın DJB2 optimizeli hash fonksiyonunu bağlıyoruz
            _walletsHashTable = new HashTable<string, BatuhanWalletNode>(
                initialCapacity: hedefCuzdanSayisi * 2, 
                hashFunc: WalletHashFunctions.HashDJB2
            );

            // İşlem ID'leri için Hash Table oluşturuyoruz
            _transactionsHashTable = new HashTable<string, BatuhanTransactionEdge>(initialCapacity: 200);

            InitializeDatabase(hedefCuzdanSayisi);
        }

        // =====================================================
        // SINGLETON ERİŞİMİ — Thread-safe
        // =====================================================
        public static AliSyntheticDataGenerator Instance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    lock (_lock)
                    {
                        if (_sharedInstance == null)
                            _sharedInstance = new AliSyntheticDataGenerator();
                    }
                }
                return _sharedInstance;
            }
        }

        // =====================================================
        // VERİ TABANI BAŞLATMA VE HASH TABLE DOLDURMA
        // =====================================================
        public void InitializeDatabase(int hedefCuzdanSayisi = 20)
        {
            if (IsInitialized) return;

            // 1. Cüzdan düğümlerini üret
            _walletsTable = GenerateWallets(hedefCuzdanSayisi);

            // YENİ: Üretilen cüzdanları O(1) erişim için Hash Table'a at
            _walletsHashTable.Clear();
            foreach (var wallet in _walletsTable)
            {
                _walletsHashTable.Set(wallet.Address, wallet);
            }

            // 2. Senaryo alt paketlerini üret
            var zincirIslemleri  = GenerateChainFlow(_walletsTable);
            var borsaIslemleri   = GenerateExchangeScenario(_walletsTable);
            var donguIslemleri   = GenerateCycleScenario(_walletsTable);
            var rastgeleIslemler = GenerateRandomTransactions(_walletsTable, 20);

            // 3. Tüm senaryoları ana tabloda birleştir
            _transactionsTable.Clear();
            _transactionsTable.AddRange(zincirIslemleri);
            _transactionsTable.AddRange(borsaIslemleri);
            _transactionsTable.AddRange(donguIslemleri);
            _transactionsTable.AddRange(rastgeleIslemler);

            // YENİ: Tüm işlemleri O(1) erişim için Hash Table'a at
            _transactionsHashTable.Clear();
            foreach (var transaction in _transactionsTable)
            {
                _transactionsHashTable.Set(transaction.TransactionId, transaction);
                
                // Borsa senaryosundaki borsa cüzdanını da Hash Table'da yoksa ekleyelim
                if (!_walletsHashTable.ContainsKey(transaction.ToAddress))
                {
                    _walletsHashTable.Set(transaction.ToAddress, transaction.To);
                }
            }

            // Senaryo partlarını da sakla
            _chainFlowPart = zincirIslemleri;
            _exchangePart  = borsaIslemleri;
            _cyclePart     = donguIslemleri;
            _randomPart    = rastgeleIslemler;
        }

        // =====================================================
        // WALLET ÜRETİMİ
        // =====================================================
        public List<BatuhanWalletNode> GenerateWallets(int count)
        {
            var wallets = new List<BatuhanWalletNode>(count);
            for (int i = 0; i < count; i++)
            {
                wallets.Add(new BatuhanWalletNode($"WALLET_{i + 1}"));
            }
            return wallets;
        }

        // =====================================================
        // RASTGELE TRANSACTION ÜRETİMİ
        // =====================================================
        public List<BatuhanTransactionEdge> GenerateRandomTransactions(List<BatuhanWalletNode> wallets, int transactionCount)
        {
            var transactions = new List<BatuhanTransactionEdge>(transactionCount);
            if (wallets == null || wallets.Count < 2) return transactions;

            for (int i = 0; i < transactionCount; i++)
            {
                var fromWallet = wallets[_random.Next(wallets.Count)];
                BatuhanWalletNode toWallet;

                do { toWallet = wallets[_random.Next(wallets.Count)]; }
                while (fromWallet.Address == toWallet.Address);

                decimal amount = Convert.ToDecimal(_random.Next(1, 5000));
                decimal fee    = Convert.ToDecimal(_random.Next(1, 50));

                transactions.Add(new BatuhanTransactionEdge(fromWallet, toWallet, amount, fee));
            }

            return transactions;
        }

        // =====================================================
        // ZİNCİRLEME FON AKIŞI (A -> B -> C -> D)
        // =====================================================
        public List<BatuhanTransactionEdge> GenerateChainFlow(List<BatuhanWalletNode> wallets)
        {
            var transactions = new List<BatuhanTransactionEdge>();
            if (wallets == null || wallets.Count < 2) return transactions;

            for (int i = 0; i < wallets.Count - 1; i++)
            {
                decimal amount = Convert.ToDecimal(_random.Next(100, 1000));
                decimal fee    = Convert.ToDecimal(_random.Next(1, 10));
                transactions.Add(new BatuhanTransactionEdge(wallets[i], wallets[i + 1], amount, fee));
            }

            return transactions;
        }

        // =====================================================
        // EXCHANGE SENARYOSU (Çok sayıda wallet -> tek merkez)
        // =====================================================
        public List<BatuhanTransactionEdge> GenerateExchangeScenario(List<BatuhanWalletNode> wallets)
        {
            var transactions = new List<BatuhanTransactionEdge>();
            if (wallets == null || wallets.Count == 0) return transactions;

            var exchangeWallet = new BatuhanWalletNode("BINANCE_EXCHANGE");

            foreach (var wallet in wallets)
            {
                decimal amount = Convert.ToDecimal(_random.Next(500, 10000));
                decimal fee    = Convert.ToDecimal(_random.Next(1, 25));
                transactions.Add(new BatuhanTransactionEdge(wallet, exchangeWallet, amount, fee));
            }

            return transactions;
        }

        // =====================================================
        // CYCLE SENARYOSU (A -> B -> C -> A)
        // =====================================================
        public List<BatuhanTransactionEdge> GenerateCycleScenario(List<BatuhanWalletNode> wallets)
        {
            var transactions = new List<BatuhanTransactionEdge>();
            if (wallets == null || wallets.Count < 3) return transactions;

            transactions.Add(new BatuhanTransactionEdge(wallets[0], wallets[1], 100m, 2m));
            transactions.Add(new BatuhanTransactionEdge(wallets[1], wallets[2], 200m, 4m));
            transactions.Add(new BatuhanTransactionEdge(wallets[2], wallets[0], 300m, 6m));

            return transactions;
        }

        // =====================================================
        // GRAPH VE DİĞER YARDIMCI METOTLAR
        // =====================================================
        // Not: DirectedGraph sınıfın varsa bu metot çalışır, yoksa burası senin asıl projene göre şekillenir.
        public void LoadTransactionsIntoGraph(dynamic graph, List<BatuhanTransactionEdge> transactions)
        {
            foreach (var transaction in transactions)
            {
                graph.BatuhanAddEdge(transaction);
            }
        }

        public List<BatuhanTransactionEdge> GenerateLargeScaleDataset(List<BatuhanWalletNode> wallets, int transactionCount)
        {
            return GenerateRandomTransactions(wallets, transactionCount);
        }

        public List<BatuhanTransactionEdge> GenerateCompleteTestDataset(List<BatuhanWalletNode> wallets)
        {
            var allTransactions = new List<BatuhanTransactionEdge>();
            if (wallets == null || wallets.Count < 3) return allTransactions;

            allTransactions.AddRange(GenerateRandomTransactions(wallets, 50));
            allTransactions.AddRange(GenerateChainFlow(wallets));
            allTransactions.AddRange(GenerateExchangeScenario(wallets));
            allTransactions.AddRange(GenerateCycleScenario(wallets));

            return allTransactions;
        }
    }
}