using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainAnalysis.DataStructures
{
    // ============================================================
    // HASH TABLE - Karma Tablo Implementasyonu
    // Proje 4: Blokzincir İşlem Ağlarının Analizi
    // Faz 1: Zorunlu Veri Yapıları
    // ============================================================
    // Bu implementasyon:
    // - Hazır kütüphane KULLANMAZ (System.Collections.Generic hariç)
    // - O(1) ortalama erişim süresi hedefler
    // - Separate Chaining ile collision yönetimi yapar
    // - Cüzdan adresleri ve işlem ID'leri için optimize edilmiştir
    // ============================================================

    // ============================================================
    // HASH TABLE ANA YAPISI
    // ============================================================

    /// <summary>
    /// İç bucket yapısı - zincirleme için kullanılır
    /// </summary>
    internal class HashEntry<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public HashEntry(TKey key, TValue value) { Key = key; Value = value; }
    }

    /// <summary>
    /// Karma Tablo (Hash Table) - Blokzincir Analizi için
    /// 
    /// Özellikler:
    /// - O(1) ortalama erişim süresi
    /// - Separate Chaining (zincirleme) ile collision çözümü
    /// - Otomatik dinamik genişleme (load factor threshold)
    /// - Collision istatistikleri takibi
    /// </summary>
    public class HashTable<TKey, TValue>
    {
        private List<HashEntry<TKey, TValue>>[] _buckets;
        private readonly Func<TKey, int>? _hashFunc;
        private readonly IEqualityComparer<TKey> _comparer;
        private int _count;
        private const double LoadFactorThreshold = 0.75;

        // ============================================================
        // PERFORMANS VE İSTATİSTİK İZLEME
        // ============================================================
        private int _totalCollisions;      // Toplam collision sayısı
        private int _totalResizes;         // Kaç kez genişleme yapıldı
        private int _totalRehashOperations; // Kaç kez rehash yapıldı

        /// <summary>
        /// Hash Tablo istatistikleri - performans analizi için
        /// </summary>
        public class HashTableStats
        {
            public int Count { get; set; }
            public int BucketCount { get; set; }
            public int TotalCollisions { get; set; }
            public int TotalResizes { get; set; }
            public double LoadFactor => BucketCount > 0 ? (double)Count / BucketCount : 0;
            public double AverageChainLength => BucketCount > 0 ? (double)Count / BucketCount : 0;
            public int MaxChainLength { get; set; }
            public double TheoreticalO1Access => LoadFactor < 1.0 ? 1.0 + (AverageChainLength - 1) * 0.5 : 1.0 + AverageChainLength;
        }

        public int Count => _count;

        /// <summary>
        /// Mevcut istatistikleri döndürür
        /// </summary>
        public HashTableStats GetStats()
        {
            var stats = new HashTableStats
            {
                Count = _count,
                BucketCount = _buckets.Length,
                TotalCollisions = _totalCollisions,
                TotalResizes = _totalResizes,
                MaxChainLength = 0
            };

            foreach (var bucket in _buckets)
            {
                if (bucket.Count > stats.MaxChainLength)
                    stats.MaxChainLength = bucket.Count;
            }

            return stats;
        }

        // ============================================================
        // KURUCU METOTLAR
        // ============================================================

        public HashTable(int initialCapacity = 16, Func<TKey, int>? hashFunc = null, IEqualityComparer<TKey>? comparer = null)
        {
            if (initialCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            _buckets = new List<HashEntry<TKey, TValue>>[initialCapacity];
            for (int i = 0; i < _buckets.Length; i++) _buckets[i] = new List<HashEntry<TKey, TValue>>();
            _hashFunc = hashFunc;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _count = 0;
            _totalCollisions = 0;
            _totalResizes = 0;
            _totalRehashOperations = 0;
        }

        // ============================================================
        // HASH FONKSİYONU
        // ============================================================

        /// <summary>
        /// Anahtarı hash indeksine dönüştürür
        /// O(1) işlem - string için optimize edilmiş
        /// </summary>
        private int GetIndex(TKey key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            int hash = _hashFunc != null ? _hashFunc(key) : key.GetHashCode();
            // Ensure non-negative and map to bucket array
            return (hash & 0x7fffffff) % _buckets.Length;
        }

        /// <summary>
        /// Test ve debug için hash indeksini döndürür
        /// </summary>
        public int GetIndexForKey(TKey key) => GetIndex(key);

        // ============================================================
        // OPERASYONLAR
        // ============================================================

        /// <summary>
        /// Yeni bir anahtar-değer çifti ekler
        /// O(1) ortalama - collision durumunda O(k) where k = zincir uzunluğu
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            int idx = GetIndex(key);
            var bucket = _buckets[idx];

            // Collision kontrolü - aynı buckette başka eleman varsa
            if (bucket.Count > 0)
            {
                _totalCollisions++;
            }

            // Mevcut anahtar var mı kontrol et
            foreach (var e in bucket)
            {
                if (_comparer.Equals(e.Key, key))
                    throw new ArgumentException("An element with the same key already exists.", nameof(key));
            }

            bucket.Add(new HashEntry<TKey, TValue>(key, value));
            _count++;

            // Load factor aşılırsa genişle
            if ((double)_count / _buckets.Length > LoadFactorThreshold)
                Resize(_buckets.Length * 2);
        }

        /// <summary>
        /// Değer erişimi - O(1) ortalama
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int idx = GetIndex(key);
            var bucket = _buckets[idx];

            foreach (var e in bucket)
            {
                if (_comparer.Equals(e.Key, key))
                {
                    value = e.Value;
                    return true;
                }
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Anahtar siler - O(1) ortalama
        /// </summary>
        public bool Remove(TKey key)
        {
            int idx = GetIndex(key);
            var bucket = _buckets[idx];

            for (int i = 0; i < bucket.Count; i++)
            {
                if (_comparer.Equals(bucket[i].Key, key))
                {
                    bucket.RemoveAt(i);
                    _count--;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Ekle veya güncelle (upsert) - O(1) ortalama
        /// </summary>
        public void Set(TKey key, TValue value)
        {
            int idx = GetIndex(key);
            var bucket = _buckets[idx];

            // Collision takibi
            if (bucket.Count > 0)
            {
                _totalCollisions++;
            }

            foreach (var e in bucket)
            {
                if (_comparer.Equals(e.Key, key))
                {
                    e.Value = value;
                    return;
                }
            }

            bucket.Add(new HashEntry<TKey, TValue>(key, value));
            _count++;

            if ((double)_count / _buckets.Length > LoadFactorThreshold)
                Resize(_buckets.Length * 2);
        }

        /// <summary>
        /// Anahtar var mı kontrolü - O(1) ortalama
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            int idx = GetIndex(key);
            var bucket = _buckets[idx];

            foreach (var e in bucket)
                if (_comparer.Equals(e.Key, key)) return true;

            return false;
        }

        // ============================================================
        // INDEXER
        // ============================================================

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value)) return value;
                throw new KeyNotFoundException();
            }
            set => Set(key, value);
        }

        // ============================================================
        // BÜYÜME VE YENİDEN HASH
        // ============================================================

        /// <summary>
        /// Tabloyu yeniden boyutlandırır - O(n) çünkü tüm elemanları yeniden hash eder
        /// </summary>
        private void Resize(int newCapacity)
        {
            var oldBuckets = _buckets;
            _buckets = new List<HashEntry<TKey, TValue>>[newCapacity];

            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = new List<HashEntry<TKey, TValue>>();

            _count = 0;
            _totalResizes++;

            foreach (var bucket in oldBuckets)
            {
                foreach (var e in bucket)
                {
                    int idx = GetIndex(e.Key);
                    _buckets[idx].Add(new HashEntry<TKey, TValue>(e.Key, e.Value));
                    _count++;
                    _totalRehashOperations++;
                }
            }
        }

        /// <summary>
        /// Tabloyu temizler - O(n)
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i].Clear();
            _count = 0;
        }

        // ============================================================
        // DEBUG VE ANALİZ
        // ============================================================

        /// <summary>
        /// Debug için bucket dağılımını gösterir
        /// </summary>
        public void PrintDistribution()
        {
            Console.WriteLine("\n=== Hash Table Bucket Dağılımı ===");
            Console.WriteLine($"Toplam eleman: {_count}");
            Console.WriteLine($"Bucket sayısı: {_buckets.Length}");
            Console.WriteLine($"Toplam collision: {_totalCollisions}");
            Console.WriteLine($"Toplam resize: {_totalResizes}");
            Console.WriteLine($"Ortalama chain uzunluğu: {(double)_count / _buckets.Length:F2}");
            Console.WriteLine();

            for (int i = 0; i < _buckets.Length; i++)
            {
                Console.Write($"Bucket[{i}]: ");
                if (_buckets[i].Count == 0)
                    Console.WriteLine("(boş)");
                else
                {
                    Console.WriteLine($"[{_buckets[i].Count} eleman] ");
                    foreach (var e in _buckets[i])
                        Console.WriteLine($"    -> {e.Key}");
                }
            }
        }
    }

    // ============================================================
    // ÖZEL STRING HASH FONKSİYONLARI
    // Cüzdan adresleri için optimize edilmiş
    // ============================================================

    /// <summary>
    /// Cüzdan adresleri için özel hash fonksiyonları
    /// Farklı dağılım özelliklerine sahip birden fazla fonksiyon
    /// </summary>
    public static class WalletHashFunctions
    {
        /// <summary>
        /// DJB2 Hash - Bitcoin adresleri için yaygın
        /// </summary>
        public static int HashDJB2(string address)
        {
            int hash = 5381;
            foreach (char c in address)
                hash = ((hash << 5) + hash) + c; // hash * 33 + c
            return hash;
        }

        /// <summary>
        /// FNV-1a Hash - iyi dağılım sağlar
        /// </summary>
        public static int HashFNV1a(string address)
        {
            uint hash = 2166136261;
            foreach (char c in address)
            {
                hash ^= c;
                hash *= 16777619;
            }
            return (int)hash;
        }

        /// <summary>
        /// MD5 tabanlı hash - Sabit uzunlukta çıktı
        /// Not: Kriptografik güvenlik için değil, sadece dağılım için
        /// </summary>
        public static int HashMD5Based(string address)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(address);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                // İlk 4 byte'ı int'e çevir
                return BitConverter.ToInt32(hashBytes, 0);
            }
        }

        /// <summary>
        /// Basit string hash - Hızlı ama daha az dağılım
        /// </summary>
        public static int HashSimple(string address)
        {
            unchecked
            {
                int hash = 17;
                foreach (char c in address)
                    hash = hash * 31 + c;
                return hash;
            }
        }
    }
}
