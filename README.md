# Hash Table Implementasyonu - Proje 4: Blokzincir İşlem Ağları

## 📋 Genel Bakış

Bu proje, blokzincir sistemlerindeki işlem verilerini analiz etmek için tasarlanmış bir veri yapıları implementasyonudur. **Faz 1: Zorunlu Veri Yapıları** kapsamında Karma Tablo (Hash Table) implementasyonu içerir.

> ⚠️ **Not:** Bu implementasyonda System.Collections.Generic harici hazır kütüphane KULLANILMAMIŞTIR. Tüm yapılar temel C# bilgisiyle sıfırdan implement edilmiştir.

---

## 📁 Dosya Yapısı

```
Veri Yapıları Proje/
├── HashTable.cs      # Hash Table implementasyonu
├── Program.cs        # Demo ve testler
├── prompts.txt       # AI prompt kayıtları
└── README.md         # Proje dokümantasyonu
```

---

## 📄 HashTable.cs - Detaylı Açıklama

### Amaç
Cüzdan adresleri ve işlem ID'leri için **O(1) ortalama erişim süresi** hedefleyen bir karma tablo implementasyonu.

### Temel Özellikler

| Özellik | Değer |
|---------|-------|
| Erişim Süresi | O(1) ortalama |
| Collision Yönetimi | Separate Chaining (Zincirleme) |
| Dinamik Genişleme | Evet (Load Factor > 0.75) |
| Hash Fonksiyonu | Özelleştirilebilir |

### Sınıflar

#### 1. `WalletNode` - Cüzdan Temsili
```csharp
public class WalletNode
{
    public string Address { get; set; }  // Cüzdan adresi (benzersiz anahtar)
    public decimal Balance { get; set; } // Bakiye
}
```
Blokzincir ağındaki bir düğümü (cüzdanı) temsil eder. HashTable'da **anahtar (key)** olarak kullanılır.

#### 2. `Transaction` - İşlem Verisi
```csharp
public class Transaction
{
    public string TransactionId { get; set; }  // Benzersiz işlem ID'si
    public string FromAddress { get; set; }     // Gönderen adres
    public string ToAddress { get; set; }       // Alıcı adres
    public decimal Amount { get; set; }         // Transfer miktarı
    public DateTime Timestamp { get; set; }     // Zaman damgası
}
```
Bir para transferini temsil eder. HashTable'da **değer (value)** olarak saklanır.

#### 3. `HashTable<TKey, TValue>` - Ana Veri Yapısı
```csharp
public class HashTable<TKey, TValue>
{
    // Public Metodlar:
    // - Add(key, value)     : Yeni anahtar-değer ekle
    // - TryGetValue(key)     : Değer bul (O(1))
    // - Remove(key)          : Anahtar sil (O(1))
    // - Set(key, value)      : Ekle veya güncelle (upsert)
    // - ContainsKey(key)     : Anahtar var mı? (O(1))
    // - this[key]            : Indexer erişimi
    // - GetStats()           : Performans istatistikleri
    // - Clear()              : Tüm veriyi temizle
}
```

### Collision (Çakışma) Yönetimi

**Separate Chaining** yöntemi kullanılır:

```
Bucket[0]  → [entry1] → [entry2] → null
Bucket[1]  → null
Bucket[2]  → [entry3] → null
Bucket[3]  → null
...
```

Aynı indekse denk gelen elemanlar bir linked list (zincir) oluşturur.

### Hash Fonksiyonları

`WalletHashFunctions` sınıfı dört farklı hash fonksiyonu içerir:

| Fonksiyon | Açıklama | Avantaj |
|-----------|----------|---------|
| `HashDJB2` | Bitcoin adresleri için yaygın | Hızlı |
| `HashFNV1a` | İyi dağılım sağlar | Düşük collision |
| `HashMD5Based` | Sabit uzunlukta çıktı | Tutarlı |
| `HashSimple` | Basit çarpma yöntemi | Anlaşılır |

### Performans Metrikleri

`HashTableStats` sınıfı aracılığıyla şu metrikler takip edilir:

```csharp
public class HashTableStats
{
    public int Count { get; set; }              // Toplam eleman
    public int BucketCount { get; set; }        // Bucket sayısı
    public int TotalCollisions { get; set; }     // Toplam collision
    public int TotalResizes { get; set; }        // Kaç kez genişledi
    public double LoadFactor { get; set; }       // Doluluk oranı
    public double AverageChainLength { get; set; } // Ortalama zincir uzunluğu
    public int MaxChainLength { get; set; }      // En uzun zincir
}
```

---

## 📄 Program.cs - Detaylı Açıklama

### Amaç
HashTable implementasyonunu **test etmek**, **doğrulamak** ve **göstermek** için kullanılan demo programıdır.

### Test Fonksiyonları

#### 1. `BasicUsageDemo()` - Temel Kullanım
En basit kullanım senaryosunu gösterir:
- Add() ile eleman ekleme
- TryGetValue() ile değer bulma
- ContainsKey() ile varlık kontrolü
- Remove() ile eleman silme
- Indexer ile güncelleme

#### 2. `CompareHashFunctions()` - Hash Karşılaştırması
Üç farklı hash fonksiyonunu 1000 cüzdan adresi ile test eder:
- Hangi fonksiyon daha az collision üretiyor?
- Ortalama chain length karşılaştırması

#### 3. `CollisionTest()` - Collision Gösterimi
**Kötü bir hash fonksiyonu** (sadece uzunluğa bakan) ile collision'ın ne olduğunu görselleştirir. Tüm elemanlar aynı bucket'ta birikir.

#### 4. `BlockchainScenarioTest()` - Blokzincir Senaryosu
Gerçek kullanımı simüle eder:
- Cüzdan oluşturma ve saklama
- İşlem oluşturma ve saklama
- O(1) erişim testi (10000 sorgu)
- Para transferi takibi

#### 5. `LargeScaleTest()` - Büyük Ölçek Testi
10000 işlem ekleyerek:
- Ekleme performansı
- Collision oranı
- Resize davranışı
- Arama hızı (O(1) doğrulaması)

---

## 🚀 Kullanım

### Projeyi Çalıştırma

```powershell
cd "c:\Users\oguz\Desktop\Veri Yapıları Proje"
dotnet run
```

### Kendi Testini Yazma

```csharp
// Yeni bir HashTable oluştur
var ht = new HashTable<string, int>(hashFunc: WalletHashFunctions.HashFNV1a);

// Eleman ekle
ht.Add("cüzdan_adresi", 100);

// Değer bul
if (ht.TryGetValue("cüzdan_adresi", out int bakiye))
    Console.WriteLine($"Bakiye: {bakiye}");

// İstatistikleri göster
var stats = ht.GetStats();
Console.WriteLine($"Collision: {stats.TotalCollisions}, Load: {stats.LoadFactor}");
```

---

## 📊 Örnek Çıktı

```
╔══════════════════════════════════════════════════════════╗
║     BLOKZİNCİR İŞLEM AĞLARI - HASH TABLE DEMO            ║
╚══════════════════════════════════════════════════════════╝

============================================================
 TEMEL KULLANIM DEMO
============================================================

--- Ekleme ---
  Eklendi: apple=5, banana=3, orange=7

--- Erişim ---
  apple => 5

--- İstatistikler ---
  Bucket: 8, Collision: 1, Load: 0,25

============================================================
 BÜYÜK ÖLÇEK TESTİ (10000 işlem)
============================================================

--- Ekleme ---
  10000 işlem eklendi
  Geçen süre: 3ms
  Collision sayısı: 4148
  Max chain: 6

--- Arama (O(1) test) ---
  Ortalama arama: 0,0001ms
  ~O(1) erişim başarıldı!
```

---

## 🔬 Teknik Detaylar

### Hash Fonksiyonu Nasıl Çalışır?

```
Anahtar (string) → Hash Fonksiyonu → Hash Değeri → Bucket İndeksi
"0xABC123"       → HashFNV1a()     → 32847392   → 32847392 % 16 = 0
```

1. Hash fonksiyonu string'i integer'a dönüştürür
2. Mod operasyonu ile bucket sayısına.indekse mapped eder
3. Collision durumunda zincirleme devreye girer

### Load Factor ve Resize

```
Load Factor = Eleman Sayısı / Bucket Sayısı

Threshold = 0.75 (75%)

Aşılırsa:
- Bucket sayısı 2 katına çıkar
- Tüm elemanlar yeniden hash edilir (O(n))
- Sonuç: Daha az collision, daha hızlı erişim
```

### Zaman Karmaşıklığı

| Operasyon | Ortalama | En Kötü |
|-----------|----------|---------|
| Add | O(1) | O(n) |
| TryGetValue | O(1) | O(n) |
| Remove | O(1) | O(n) |
| ContainsKey | O(1) | O(n) |

---

## ✅ Ödev Gereksinimleri Karşılama

- [x] Hazır kütüphane kullanmamak
- [x] O(1) erişim süresi hedeflemek
- [x] Manuel hash fonksiyonu tasarlamak
- [x] Collision yönetimi yazmak
- [x] Performans metrikleri sunmak

---
