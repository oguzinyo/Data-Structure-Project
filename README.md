
Bu proje, bir blokzincir ağındaki işlem verilerinin bütünlüğünü ve doğruluğunu kontrol etmek amacıyla C# ile geliştirilmiş
bir Merkle Tree (Karma Ağacı) uygulamasıdır.
=======

# Proje 4: Blokzincir İşlem Ağları - Yönlü Graf Altyapısı (Faz 1)

Bu modül, blokzincir sistemlerindeki işlem verilerini sadeleştirilmiş bir graf modeli üzerinden incelemek amacıyla geliştirilmiştir. Projenin Faz 1 aşamasında, ağın temelini oluşturan yönlü graf veri yapısı ve cüzdan yönetim mekanizmaları kurulmuştur.

## 🛠 Teknik Mimari ve Veri Yapıları

Proje gereksinimleri doğrultusunda, sistemin yüksek performanslı ve güvenli çalışması için aşağıdaki yapılar tercih edilmiştir:
>>>>>>> origin/main

### 1. WalletNode (Cüzdan Düğümü)
Ağdaki her bir benzersiz cüzdan adresini temsil eden **düğüm (vertex)** yapısıdır.
* **Address (string):** Cüzdanın benzersiz kimliğidir. Blokzincir mantığına uygun olarak `private set` ile korunur ve değiştirilemez.
* **Balance (double):** Cüzdanın güncel bakiyesini tutar. Sadeleştirilmiş model gereği transferler gerçekleştikçe dinamik olarak güncellenir.

<<<<<<< HEAD

📌 Özellikler
Veri Bütünlüğü: Yaprak düğümlerdeki (leaf nodes) herhangi bir değişikliğin kök hash (Root Hash) üzerindeki etkisi gözlemlenebilir.

Hash Algoritması: Veriler, kriptografik olarak güvenli bir şekilde birleştirilerek ağaç yapısı oluşturulur.

Hızlı Doğrulama: Büyük veri setleri içinde belirli bir işlemin varlığı, tüm veri setini kontrol etmeye gerek kalmadan doğrulanabilir.



🛠 Teknik Detaylar
Dil: C# [.NET]

Mimari: İkili Ağaç (Binary Tree) yapısı üzerine kurgulanmıştır.

Dosyalar:

MerkleTreeFunc.cs: Ağacın oluşturulması ve hash hesaplama mantığını içeren temel sınıf.

Demo.cs: Sistemin nasıl çalıştığını gösteren test ve senaryo dosyası.
=======
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

