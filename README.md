# Proje 4: Blokzincir İşlem Ağları - Yönlü Graf Altyapısı (Faz 1)

Bu modül, blokzincir sistemlerindeki işlem verilerini sadeleştirilmiş bir graf modeli üzerinden incelemek amacıyla geliştirilmiştir. Projenin Faz 1 aşamasında, ağın temelini oluşturan yönlü graf veri yapısı ve cüzdan yönetim mekanizmaları kurulmuştur.

## 🛠 Teknik Mimari ve Veri Yapıları

Proje gereksinimleri doğrultusunda, sistemin yüksek performanslı ve güvenli çalışması için aşağıdaki yapılar tercih edilmiştir:

### 1. WalletNode (Cüzdan Düğümü)
Ağdaki her bir benzersiz cüzdan adresini temsil eden **düğüm (vertex)** yapısıdır.
* **Address (string):** Cüzdanın benzersiz kimliğidir. Blokzincir mantığına uygun olarak `private set` ile korunur ve değiştirilemez.
* **Balance (double):** Cüzdanın güncel bakiyesini tutar. Sadeleştirilmiş model gereği transferler gerçekleştikçe dinamik olarak güncellenir.

### 2. TransactionEdge (İşlem Kenarı)
İki cüzdan arasındaki para transferini mühürleyen **yönlü kenar (edge)** yapısıdır.
* **Yönlü Akış:** İşlem, gönderen (From) ve alıcı (To) düğümlerini referans alarak paranın akış yönünü tanımlar.
* **Metadata:** Her işlem miktar (Amount) ve işlemin gerçekleştiği anı gösteren zaman damgası (Timestamp) bilgilerini taşır.
* **Doğrulama:** Negatif miktarlı veya tanımsız düğümler arası transferleri engelleyen güvenlik kontrollerine sahiptir.

### 3. BlockchainGraph (Graf Yönetim Merkezi)
Tüm ağın koordinasyonunu sağlayan ana sınıftır.
* **Komşuluk Listesi (Adjacency List):** Bellek tasarrufu sağlamak amacıyla (Sparse Graph yapısı için) tercih edilmiştir.
* **Thread-Safety (Eşzamanlılık):** `ConcurrentDictionary` ve `ConcurrentBag` koleksiyonları kullanılarak, çoklu iş parçacıklarının (thread) aynı anda veri yazması durumunda oluşabilecek çakışmalar (race conditions) önlenmiştir.
* **Hızlı Erişim:** Cüzdan adresleri üzerinden yapılan sorgular, Hash Table mantığıyla ortalama **O(1)** sürede sonuçlanır.

## 📈 Algoritmik Analiz (Big-O)

Sistemin ölçeklenebilirliği için aşağıdaki karmaşıklık değerleri hedeflenmiştir:

| İşlem | Zaman Karmaşıklığı | Açıklama |
| :--- | :--- | :--- |
| **Cüzdan Ekleme** | $O(1)$ | Hash Table (Dictionary) kullanımı sayesinde doğrudan erişim. |
| **İşlem (Kenar) Kaydı** | $O(1)$ | Gönderen cüzdanın komşuluk listesine doğrudan ekleme. |
| **Cüzdan Bulma** | $O(1)$ | Benzersiz adres anahtarı ile hızlı sorgulama. |
| **Hafıza Kullanımı** | $O(V + E)$ | Toplam düğüm (V) ve kenar (E) sayısıyla orantılı verimli alan kullanımı. |

## 🚀 Entegrasyon Notları

Bu altyapı, projenin sonraki fazlarında yer alan aşağıdaki görevler için temel oluşturur:
* **Faz 2 (Algoritmalar):** BFS ve DFS algoritmaları, bu sınıftaki `AdjacencyList` üzerinden fon akışı analizi yapacaktır.
* **Faz 3 (Görselleştirme):** Arayüz katmanı, düğüm boyutlarını ve kenar kalınlıklarını belirlemek için buradaki bakiye ve miktar verilerini okuyacaktır.

## ✅ Karşılanan Proje Gereksinimleri ve Uygulama Yöntemi
Bu modül kapsamında, proje föyünde belirtilen aşağıdaki kriterler başarıyla karşılanmıştır:

### B.1. Takım Çalışması ve Teknolojik Altyapı

-   **Eşzamanlılık ve Mikroservis Yaklaşımı:** Yapay zeka motorunun ana bellekten bağımsız çalışabilmesi için `ConcurrentDictionary` ve `ConcurrentBag` kullanılarak **Thread-safe** bir altyapı kurulmuştur.
    
-   **Versiyon Kontrolü (Git):** Doğrudan `main` dalına müdahale edilmeden, `feature-directed-graph` dalı üzerinden izole bir geliştirme süreci yürütülmüştür.
    

### B.3. Teslim Kuralları ve Değerlendirme

-   **İsimlendirme Şartı:** Veritabanı ve fonksiyon isimlendirmelerinde (WalletNode, TransactionEdge, BlockchainGraph vb.) **Türkçe karakter içermeyen** ve global standartlara uygun bir format kullanılmıştır.
    

### Proje Konusu: Blokzincir İşlem Ağları Analizi

-   **Yönlü Graf Modellemesi:** Cüzdan adresleri düğüm (vertex), para transferleri ise yönlü kenarlar (edge) olarak modellenmiştir.
    
-   **Kenar Özellikleri:** Her transfer işlemi için miktar (amount) ve zaman (timestamp) bilgisi veri modeline dahil edilmiştir.
    
-   **Döngü (Cycle) Desteği:** Sistem tasarımı, blokzincir ağlarında sık rastlanan döngüsel transferlere (A -> B -> A) izin verecek esnekliktedir.
    
-   **Karma Tablo (Hash Table) Kullanımı:** Cüzdan adreslerine erişim süresini ortalama **O(1)** seviyesine çekmek için Hash Table tabanlı `ConcurrentDictionary` yapısı entegre edilmiştir.
    
-   **Bakiye Hesaplama:** Sadeleştirilmiş model gereği, işlem anında gönderen ve alıcı bakiyeleri otomatik olarak güncellenecek şekilde kodlanmıştır.
    

### Raporlama ve Analiz

-   **Big-O Analizi:** Tasarlanan veri yapılarının zaman ve uzay karmaşıklığı analizleri teknik dökümantasyona eklenmiştir.
---
*Bu döküman, iş bölümünde Faz 1'in Yönlü Graf kısmından sorumlu olan Batuhan Özdemir'in sorumlulukları kapsamında hazırlanan kodların teknik dökümantasyonudur.*

# Blokzincir Islem Aglari Analizi

Bu proje, blokzincir sistemlerindeki islem verilerini sade bir model uzerinden analiz etmek icin hazirlanmis bir Veri Yapilari projesidir. Cuzdan adresleri graf dugumu, para transferleri ise miktar ve zaman bilgisi tasiyan yonlu kenar olarak modellenir.

## Proje Amaci

Projenin temel amaci, kripto para islem agini temel veri yapilari ile temsil etmek ve terminal uzerinden calisan bir Faz 1 demosu sunmaktir.

Faz 1 kapsaminda su yapilar calisir durumdadir:

- Yonlu graf ile cuzdanlar arasi transfer agi modellenir.
- Hash table ile cuzdan adreslerine ve islem ID'lerine ortalama O(1) erisim saglanir.
- Merkle tree ile islem verilerinden Merkle Root uretilir ve veri butunlugu dogrulanir.
- Queue ile BFS dolasimi yapilir.
- Stack ile DFS dolasimi yapilir.
- Gelen, giden ve net fon akisi hesaplanir.

## Proje Mimarisi

```text
BlockchainAnalysis.App/
  Program.cs

BlockchainAnalysis.Core/
  IGraph.cs
  IHashTable.cs

BlockchainAnalysis.DataStructures/
  BlockchainGraph.cs
  CustomQueue.cs
  CustomStack.cs
  DirectedGraph.cs
  HashTable.cs
  MerkleTree.cs

BlockchainAnalysis.Models/
  GraphNode.cs
  TransactionEdge.cs
  Transaction.cs
  Wallet.cs
  WalletNode.cs

BlockchainAnalysis.slnx
Dockerfile
docker-compose.yml
```

## Katmanlar

- `BlockchainAnalysis.App`: Konsol uygulamasi ve Faz 1 demo akisinin basladigi katman.
- `BlockchainAnalysis.Core`: Veri yapilari icin ortak arayuzleri barindirir.
- `BlockchainAnalysis.DataStructures`: Projede kullanilan temel veri yapilarini icerir.
- `BlockchainAnalysis.Models`: Cuzdan ve islem modellerini icerir.

## Veri Yapilari

### BlockchainGraph

`BlockchainGraph`, kripto islem agini node-edge modeliyle yonlu graf olarak temsil eder.

- Dugumler `GraphNode` sinifi ile temsil edilir.
- Kenarlar `TransactionEdge` sinifi ile temsil edilir.
- Kenarlarda transfer miktari ve zaman bilgisi bulunur.
- Graf dongu icerebilir. Ornek: `0x7788 -> 0xA1B2` transferi ile para akisi tekrar baslangic adresine donebilir.

Node ve edge iliskisi adjacency list ile kurulur:

```text
BlockchainGraph
  nodes[address] -> GraphNode
  adjacencyList[fromAddress] -> List<TransactionEdge>
```

Ornek:

```text
GraphNode(0xA1B2)
  Edge: 0xA1B2 -> 0xC3D4 | Amount: 12,50
  Edge: 0xA1B2 -> 0xE5F6 | Amount: 4,25
```

Demo ciktisinda her cuzdan icin su bilgiler gosterilir:

```text
0xA1B2 | Incoming: 0,50 | Outgoing: 16,75 | Net Flow: -16,25
```

Buradaki `Net Flow`, gercek blokzincir bakiyesi degil, basitlestirilmis modeldeki `gelen transferler - giden transferler` sonucudur.

### Hash Table

`HashTable<TKey, TValue>`, separate chaining yaklasimi ile collision yonetimi yapan generic bir hash table implementasyonudur.

Ozellikler:

- Ortalama O(1) ekleme ve arama hedeflenir.
- Cuzdan adresleri icin kullanilir.
- Islem ID'leri icin kullanilir.
- Load factor, collision ve bucket istatistikleri terminalde gosterilir.

### Merkle Tree

`MerkleTree`, islem payload'larini SHA-256 ile hashler ve ikili agac yapisinda birlestirerek Merkle Root uretir.

Demo akisinda:

- Her islem icin hash uretilir.
- Merkle Root hesaplanir.
- Orijinal veri dogrulanir.
- Veri degistirildiginde dogrulamanin basarisiz oldugu gosterilir.

### Queue ve Stack

Projede BFS ve DFS icin custom veri yapilari kullanilir.

- `CustomQueue<T>`: BFS icin FIFO mantigiyla calisir.
- `CustomStack<T>`: DFS icin LIFO mantigiyla calisir.

## Terminal Demo Akisi

Uygulama calistirildiginda su bolumler terminalde gosterilir:

1. Sentetik islem verileri
2. Hash table ile cuzdan ve islem ID erisimi
3. Yonlu graf ile transfer agi
4. Queue ve Stack ile graf dolasimi
5. Merkle tree ile islem butunlugu
6. Faz 1 kontrol ozeti

Ornek graf ciktisi:

```text
0xA1B2 | Incoming: 0,50 | Outgoing: 16,75 | Net Flow: -16,25
  -> 0xC3D4 | Amount: 12,50 | Time: 12:25:11
  -> 0xE5F6 | Amount: 4,25 | Time: 12:25:11
```

Ornek dolasim ciktisi:

```text
BFS (Queue) from 0xA1B2: 0xA1B2 -> 0xC3D4 -> 0xE5F6 -> 0x7788
DFS (Stack) from 0xA1B2: 0xA1B2 -> 0xC3D4 -> 0x7788 -> 0xE5F6
```

## Gereksinimler

- .NET SDK 10.0
- Docker Desktop, Docker ile calistirmak istenirse

Kurulu SDK'lari kontrol etmek icin:

```bash
dotnet --list-sdks
```

## Calistirma

Projeyi derlemek icin:

```bash
dotnet build BlockchainAnalysis.slnx
```

Konsol uygulamasini calistirmak icin:

```bash
dotnet run --project BlockchainAnalysis.App/BlockchainAnalysis.App.csproj
```

Windows PowerShell icin alternatif komut:

```powershell
dotnet run --project "BlockchainAnalysis.App\BlockchainAnalysis.App.csproj"
```

## Docker Ile Calistirma

Docker uzerinden calistirmak icin:

```bash
docker-compose up --build
```

Container'i durdurmak icin:

```bash
docker-compose down
```

## Entegre Edilen Branch Icerikleri

- `setup-architecture`: Solution, proje klasorleri, Docker dosyalari ve temel modeller.
- `feature/hash-table`: Generic hash table implementasyonu ve wallet hash fonksiyonlari.
- `feature-directed-graph`: Graf dugumu olarak kullanilacak wallet node modeli.
- `feature-merkle-tree`: Merkle agaci baslangic dugum modeli.

## Faz 1 Durumu

- `[OK]` Directed Graph
- `[OK]` BlockchainGraph node-edge modeli
- `[OK]` Merkle Tree
- `[OK]` Hash Table
- `[OK]` Queue
- `[OK]` Stack

## Takim Uyeleri

- OGUZHAN HEKIMOGLU
- MEHMET OGUZHAN TANRIVERDI
- BATUHAN OZDEMIR
- ALI KADIR OZYASAR
- UMMET ERKAN

## Notlar

- `bin/`, `obj/` ve `.vs/` klasorleri repoya dahil edilmez.
- Terminalde gosterilen bakiye degeri `Net Flow` olarak hesaplanir.
- Bu proje su an konsol uygulamasi olarak calisir; Faz 3 arayuz gereksinimleri henuz uygulanmamistir.

