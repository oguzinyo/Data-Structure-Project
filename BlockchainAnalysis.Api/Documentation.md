
**BlockchainAnalysis.Api Katmanı Kapsamlı Mimari Analizi**

**1. Katmanın Temel Amacı ve Mimarisi**

BlockchainAnalysis.Api dizini, projede geliştirilen veri yapıları ve algoritmaların dış dünyaya açıldığı Sunum (Presentation) ve Entegrasyon katmanıdır. ASP.NET Core tabanlı bir RESTful Web API olarak tasarlanmıştır. Bu katman, alt katmanlardaki (Core, DataStructures, Models) karmaşık iş mantığını ve hesaplama motorlarını kapsüller; Frontend (Angular) gibi istemcilerin ağ üzerinden tüketebileceği standart JSON formatına dönüştürür.

**2. Proje Yapılandırması ve Başlatma (Program.cs ve csproj)**

-   Hedef Platform: Proje, bellek yönetimi ve performans avantajlarından yararlanmak üzere en güncel .NET 10.0 platformunu hedeflemektedir.
    
-   Bağımlılık Enjeksiyonu (Dependency Injection): Servis katmanı olan BlockchainService sınıfı, bellekte tek bir kopya olarak yaşayacak şekilde Singleton yaşam döngüsüyle (AddSingleton) sisteme enjekte edilmiştir. Bu yapılandırma, graf verisinin uygulama çalıştığı sürece hafızada kalmasını ve her HTTP isteğinde maliyetli graf inşasının sıfırdan yapılmamasını garanti altına alır.
    
-   CORS Politikası: Frontend uygulamasının farklı bir porttan (örneğin localhost:4200) API'ye sorunsuz erişebilmesi için "AllowFrontend" adında, tüm kökenlere ve metotlara izin veren bir Cross-Origin Resource Sharing (CORS) politikası tanımlanmıştır.
    
-   API Dokümantasyonu: Geleneksel Swagger yerine, modern ve yüksek performanslı Scalar.AspNetCore paketi kullanılarak BluePlanet temalı etkileşimli bir OpenAPI dokümantasyon arayüzü kurulmuştur.
    

**3. Merkezi Orkestrasyon (BlockchainService.cs)**

Bu sınıf, yazılım tasarımında dış dünyadaki istemciler ile iç sistemler arasındaki iletişimi tek bir noktadan yöneten Facade (Önyüz) tasarım deseninin ideal bir örneğidir.

-   Veri İlklendirme (Bootstrapping): Nesne oluşturulduğunda SyntheticDataGenerator devreye girerek 20 adet cüzdan düğümü ve aralarındaki sentetik transferleri üretir.
    
-   Graf İnşası: Üretilen veriler, ardışık döngüler yardımıyla BlockchainGraph nesnesine (düğüm ve kenar olarak) eklenir. Cüzdanlara, sistemdeki transferlerin sağlıklı çalışabilmesi için giden işlem hacmi kadar başlangıç fonu hesaplanarak (AddFunds) tanımlanır.
    
-   Motor Entegrasyonu: Core katmanında yer alan FundFlowTracker ve UmmetDynamicBalanceEngine gibi algoritmik analiz sınıfları burada örneklenerek hazırda bekletilir.
    

**4. Veri Taşıma Nesneleri (DTO - Data Transfer Objects)**

GraphDtos, MerkleDtos ve AnalysisDtos dosyalarından oluşan bu dizin, sistemin iç dinamiklerini dış dünyadan izole etmek için kritik bir katmandır.

-   Güvenlik ve Serileştirme: Nesne yönelimli ana modeller (BatuhanWalletNode vb.) senkronizasyon kilitleri (BalanceLock) veya grafiğin komşuluk listelerinden kaynaklanan döngüsel referanslar (circular reference) barındırır. DTO sınıfları ise yalnızca taşınacak saf değerleri (Id, Amount, Label) barındırarak nesnelerin güvenle JSON formatına serileştirilmesini (Serialization) sağlar.
    
-   Kompleks Yanıt Paketleme: Örneğin FlowResultDto sınıfı, dışarıdan gelen tek bir fon takibi isteğine karşılık hem BFS dolaşım sırasını, hem DFS dolaşım sırasını, hem de filtrelenmiş transfer kenarlarını (FlowEdges) tek bir çatı altında toplayarak Frontend'e zengin bir veri seti sunar.
    

**5. Denetleyiciler (Controllers - API Uç Noktaları)**

Gelen HTTP taleplerini işleyen ve BlockchainService üzerinden ilgili analizleri tetikleyen uç noktalardır.

-   AnalysisController: Sistemin en yoğun algoritmik yükünü çeken uç noktadır. Bakiye sorgulama (api/analysis/balance), ileri ve geri yönlü fon dolaşımı (api/analysis/flow), iki cüzdan arası en kısa yol (path) ve maksimum kapasite yolu analizlerini yönetir.
    
-   GraphController: D3.js tabanlı ağ görselleştirme motorunun ihtiyaç duyduğu tüm yapısal veriyi sağlar. Sistemdeki tüm düğümleri ve kenarları ayrıştırarak (veya paket halinde) sunar.
    
-   MerkleController: Spesifik bir işlemin doğrulanma rotasını (Proof Path) belirlemek üzere, tüm işlem geçmişinden Merkle ağacını inşa eder ve bu yapıyı hiyerarşik olarak (api/merkle/tree) dışarı aktarır.
    

**6. Mimari Değerlendirme ve Akademik Sonuç**

Bu katman, modern yazılım mühendisliğindeki İlgilerin Ayrılığı (Separation of Concerns) prensibine kesin bir uyum göstermektedir. HTTP rotalama ve doğrulama işlemleri Controller'larda, dış veri kopyalama işlemleri DTO'larda, iç veri çekirdeği ile orkestrasyon ise Service katmanında izole edilmiştir. Sistem ilerleyen aşamalarda bir mikroservis mimarisine geçirilmek istendiğinde, bu API katmanı hiçbir alt algoritmik kodu değiştirmeye gerek kalmadan bağımsız bir konteyner olarak ağ üzerinden yayın yapabilecek esnekliğe sahiptir.
