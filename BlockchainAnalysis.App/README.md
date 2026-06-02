# BlockchainAnalysis.App Dizini ve Konsol Katmanı Analizi

BlockchainAnalysis.App dizini, geliştirilen blokzincir veri yapıları sisteminin sunum ve test katmanını temsil etmektedir. Bu dizin, çekirdek (Core), veri yapıları (DataStructures) ve model (Models) katmanlarında inşa edilen algoritmaların doğruluğunu sınamak için tasarlanmış bağımsız bir çalıştırılabilir ortamdır.

## 1. Proje Yapılandırması: BlockchainAnalysis.App.csproj

Bu dosya, uygulamanın derleme talimatlarını ve bağımlılık hiyerarşisini tanımlar.

* Mimari Bağımlılıklar: Proje; Core, DataStructures ve Models projelerini referans almaktadır. Bu durum, sistemin İlgilerin Ayrılığı (Separation of Concerns) prensibine uygun olarak katmanlı bir yapıda tasarlandığını gösterir. Uygulama katmanı veri yapılarını sıfırdan yazmak yerine, alt katmanların sunduğu arayüzleri tüketmektedir.
* Hedef Çerçeve: Sistemin modern bellek yönetimi ve performans avantajlarından yararlanmasını sağlayan .NET 10.0 platformu hedeflenmiştir. Nullable referans tiplerinin etkinleştirilmiş olması, potansiyel boş referans (null reference) hatalarının derleme aşamasında yakalanmasına olanak tanır.

## 2. Ana Çalışma Ortamı: Program.cs

Sistemin ana çalışma mantığı, test senaryolarını ardışık olarak işleten Main ve TestFundFlowFilters metotları üzerine kuruludur.

Faz 1: Temel Veri Yapıları Entegrasyonu (Pasif Durum)

Yorum satırına alınmış olan Faz 1 bloku, projenin yapıtaşı olan özel veri yapılarının entegrasyon testlerini barındırır.

* Yönlü Graf (Directed Graph) Modeli: Cüzdanlar birer düğüm (BatuhanWalletNode), aralarındaki fon transferleri ise ağırlıklı ve yönlü kenarlar (BatuhanTransactionEdge) olarak modellenmiştir.
* Hash Tablosu (Hash Table) Analizi: Cüzdan adreslerine ve işlem kimliklerine deterministik olarak O(1) zaman karmaşıklığında erişebilmek için FNV-1a hash algoritması ile desteklenen özel bir yapı kullanılmıştır. Çakışma (Collision) ve Yük Faktörü (Load Factor) istatistiklerinin alınması, tablonun bellek verimliliğini ölçmek için ideal bir yaklaşımdır.
* Graf Dolaşımı (Graph Traversal): Cüzdanlar arası ilişkilerin tespiti için BFS (Kuyruk tabanlı) ve DFS (Yığın tabanlı) dolaşım algoritmaları test edilmiştir. Bu algoritmalar ağın zaman karmaşıklığını O(V + E) sınırlarında tutar (V: Düğüm, E: Kenar sayısı).
* Merkle Ağacı Doğrulaması: İşlem bütünlüğünü sağlamak adına AliMerkleTree sınıfı kullanılarak bir kriptografik özet ağacı inşa edilmiş ve manipüle edilmiş verilerin reddedilmesi simüle edilmiştir.

Faz 2: Fon Akış İzleyicisi (Aktif Durum)

TestFundFlowFilters metodu, geliştirilen algoritmaların blokzincir senaryolarına uygulanmasını simüle eder.

* Sentetik Ağ İnşası: Test ortamında cüzdanlar (Alice, Bob, Charlie, David) arasında senkronizasyon problemlerini önlemek ve zaman damgası doğruluğunu sağlamak amacıyla iş parçacığı bekletmesi (Thread.Sleep) kullanılarak ardışık işlemler yaratılmıştır.
* İleriye Dönük Akış (Forward Flow): Belirli bir cüzdandan çıkan tüm fonların izi, graf üzerinde ileriye doğru taranmaktadır.
* Bütçe Budaması (Pruning / Filtering): "minAmount" parametresi ile graf üzerindeki işlemlerin filtrelenmesi sağlanmıştır. Bu özellik, graf teorisinde belirli ağırlığın altındaki kenarların atlanması (pruning) mantığıyla çalışarak arama uzayını küçültür ve algoritmanın çalışma süresini optimize eder.
* Geriye Dönük Akış (Backward Flow): Hedef bir cüzdana ulaşan fonların kök kaynağını bulmak için grafın kenar yönlerinin tersine doğru işlendiği (Reverse Edge Traversal) bir algoritma çalıştırılmaktadır.

## Genel Değerlendirme

BlockchainAnalysis.App dizini, karmaşık veri yapılarını (Graflar, Merkle Ağaçları, Hash Tabloları) soyutlayarak test için basit bir arayüz sunar. Doğrudan bir veri analiz motorunun çekirdek simülasyonunu başarıyla gerçekleştirmekte ve büyük veri setleri entegre edilmeden önce sistemin algoritmik doğruluğunu kanıtlamaktadır.
