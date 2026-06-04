
# BlockchainAnalysis.DataStructures Dizini Kapsamlı Mimari Analizi

Bu dizin, projenin en alt seviyesini oluşturan ve harici koleksiyon kütüphanelerinden (System.Collections.Generic kısıtlamaları dahilinde) bağımsız olarak sıfırdan inşa edilmiş temel veri yapılarının merkezidir. Sistemdeki bellek yönetimi, algoritmik karmaşıklık optimizasyonları ve düğümler arası ilişkiler bu katmanda çözümlenmektedir.

## 1. Proje Yapılandırması (BlockchainAnalysis.DataStructures.csproj)

Bu dosya, katmanın .NET 10.0 platformunu hedeflediğini gösterir. Sistem, `Core` ve `Models` projelerini referans alarak alt katmanların sunduğu soyutlamaları (IGraph gibi) ve veri modellerini (WalletNode) somut veri yapılarına dönüştürür.

## 2. Doğrusal Bellek Yapıları: Yığın (Stack) ve Kuyruk (Queue)

Graf dolaşım algoritmalarının temel yapıtaşları olan bu iki sınıf, jenerik dizi (array) tabanlı ve dinamik boyutlandırma kapasitesine sahip olacak şekilde tasarlanmıştır.

-   **Stack.cs (LIFO - Son Giren İlk Çıkar):** Graf üzerindeki **Derinlik Öncelikli Arama (DFS)** algoritmasında düğüm takibi için kullanılır. Dizi kapasitesi dolduğunda `Resize` metodu ile boyut 2 katına çıkarılır (Amortize edilmiş zaman karmaşıklığı: O(1)). Eleman çıkarıldığında (Pop), bellek sızıntısını (memory leak) önlemek ve Çöp Toplayıcı'nın (Garbage Collector) çalışmasını kolaylaştırmak için boşalan referanslara `default!` atanır.
    
-   **Queue.cs (FIFO - İlk Giren İlk Çıkar):** **Genişlik Öncelikli Arama (BFS)**, ileriye/geriye dönük fon akışı izleme ve hedef rota analizi işlemlerinde kullanılır. Doğrusal kaydırma (O(N)) maliyetinden kaçınmak için modüler aritmetik kullanılarak Dairesel (Circular) yapıda tasarlanmıştır. Başı ve sonu (_head, _tail) dinamik olarak güncellenir ve ekleme/çıkarma operasyonları sabit zamanda (O(1)) gerçekleşir.
    

## 3. Anahtar-Değer Eşleme: Karma Tablo (Hash Table)

-   **HashTable.cs:** Sistemin O(1) ortalama erişim süresi gerektiren tüm işlemlerinin (cüzdan aramaları, işlem doğrulama, komşuluk listesi takibi) omurgasıdır.
-   **Çakışma Yönetimi (Collision Handling):** Aynı indekse denk gelen anahtarlar için Ayrı Zincirleme (Separate Chaining) yöntemi kullanılmıştır. Çakışan elemanlar aynı bucket içindeki bir bağlı listede tutulur.
    
-   **Dinamik Genişleme:** Tablodaki eleman sayısının toplam kapasiteye oranı 0.75'i (LoadFactorThreshold) aştığında, tablo boyutu iki katına çıkarılır ve mevcut tüm elemanlar yeniden hash edilerek (Rehash) O(N) zamanında yeni yerlerine dağıtılır. Bu işlem sonrasında O(1) performansı korunur.
    
-   **Optimizasyon:** İçerisinde kriptografik adreslere (Cüzdan/Transaction ID) uygun `WalletHashFunctions` statik sınıfı bulunur. DJB2 ve FNV-1a gibi string dağılımı yüksek hash fonksiyonları entegre edilmiştir.
    

## 4. Kriptografik Bütünlük: Merkle Ağacı

-   **MerkleTree.cs:** İşlem bütünlüğünün (Data Integrity) doğrulanması için kullanılan ağaç yapısıdır.
    
-   **İşleyiş:** Verilen işlem dizisi (payloads) önce yaprak düğümlere dönüştürülür. Ardından her seviyede yan yana duran iki düğümün SHA256 özetleri birleştirilerek tek bir Kök Özet (Root Hash) elde edilene kadar işlem logaritmik olarak yukarı taşınır.
    
-   **Doğrulama:** Sisteme verilen yeni bir veri setinin, beklenen kök hash ile uyumlu olup olmadığı kontrol edilir. Alt yapraklardaki tek bir bitlik değişiklik, Kök Özet'i tamamen değiştireceği için manipülasyon hızla tespit edilir.
    

## 5. Ağ Modelleme ve Graf Teorisi Mimari Analiz

Graf teorisinde bir grafı bilgisayar belleğinde saklamanın iki temel yöntemi vardır: Komşuluk Matrisi (Adjacency Matrix) ve Komşuluk Listesi (Adjacency List). Geliştirdiğiniz projede **Komşuluk Listesi Mimarisi** tercih edilmiştir.

**Komşuluk Listesi Mantığı:** Bu mimaride, graftaki her bir düğüm (cüzdan) için o düğümden çıkan kenarların (transferlerin) bir listesi tutulur. Eğer A cüzdanından B cüzdanına bir para transferi gerçekleştirilmişse, A düğümünün hedef listesine B'ye giden bu transfer kenarı eklenir.

-   **Uzay Karmaşıklığı:** V düğüm sayısı ve E kenar sayısı olmak üzere, bir komşuluk matrisi bellekte V x V boyutunda yer kaplarken, komşuluk listesi sadece O(V + E) kadar bellek harcar. Blokzincir ağları gibi milyonlarca cüzdanın bulunduğu ancak her cüzdanın ağdaki diğer tüm cüzdanlarla doğrudan transfer yapmadığı "seyrek graflarda" (sparse graphs) bu yaklaşım bellek verimliliği açısından zorunludur.
    
-   **Erişim Hızı:** Bir düğümün komşularını veya o cüzdandan çıkan işlemleri bulmak için tüm matrisi taramak gerekmez; sadece o düğüme ait listeyi dönmek yeterlidir.
    

Bu mimari hem `DirectedGraph.cs` hem de `BlockchainGraph.cs` dosyalarında doğrudan kendi yazdığınız `HashTable` yapısı üzerine kurulmuştur:

-   `DirectedGraph.cs` içinde: `private readonly HashTable<string, List<BatuhanTransactionEdge>> _adjacency;`
    
-   `BlockchainGraph.cs` içinde: `private readonly HashTable<string, List<BatuhanTransactionEdge>> _adjacencyList;`
    
Burada standart bir iki boyutlu dizi yerine anahtar-değer eşlemeli `HashTable` kullanılması, bir cüzdan adresinin komşuluk listesine O(1) ortalama zaman karmaşıklığıyla (sabit zamanda) doğrudan erişilmesini sağlar.

**DirectedGraph ve BlockchainGraph Arasındaki Farklar:** Her iki sınıf da `BlockchainAnalysis.Core` katmanındaki `IGraph` arayüzünü (interface) implemente eder ve sistemle aynı sözleşmeye bağlıdır. Ancak tasarım amaçları, içerdikleri algoritmalar ve eşzamanlılık (thread-safety) stratejileri açısından aralarında önemli farklar bulunur.

#### 1. Eşzamanlılık ve Kilitleme (Thread-Safety) Stratejisi

**Eşzamanlılık (Concurrency)**, bir sistemde birden fazla iş parçacığının (thread) aynı zaman dilimi içerisinde paylaşılan bellek kaynaklarına (örneğin projenizdeki cüzdan düğümlerine veya komşuluk listelerine) erişmesi ve işlem yürütmesidir.

**Kilitleme (Locking)** ise bu eşzamanlı erişimler sırasında birden fazla iş parçacığının aynı veriyi aynı anda değiştirmeye çalışarak veri bütünlüğünü bozmasını (Race Condition / Yarış Durumu) engellemek amacıyla, kaynağı geçici olarak sadece tek bir iş parçacığının kullanımına tahsis eden senkronizasyon mekanizmasıdır.

Projenizde `lock (_graphLock)` veya `lock (wallet.BalanceLock)` ifadeleriyle uyguladığınız bu yapı, bir thread bakiye güncellerken diğer thread'lerin beklemesini sağlayarak hatalı hesaplamaların önüne geçer.

-   **DirectedGraph:** Kilitleme mekanizmasını daha dar kapsamlı (fine-grained) ele alır. `BatuhanAddEdge` metodunda grafın genelini kilitlemek yerine, sadece o an işleme konu olan kaynak ve hedef cüzdan nesnelerinin kendi iç kilit yapılarını (`BalanceLock`) ayrı ayrı kilitler. Graf yapısının bütünü üzerinde yapısal bir kilit bulundurmaz.
    
-   **BlockchainGraph:** Tam kapsamlı (coarse-grained) ve merkezi bir eşzamanlılık sunar. Sınıfın başında `private readonly object _graphLock = new object();` tanımlanmıştır. `BatuhanAddVertex`, `BatuhanAddEdge` ve `BatuhanGetOutgoingEdges` gibi yapısal değişiklik veya okuma yapan tüm operasyonlar bu merkezi kilit ile senkronize edilir. Ayrıca `BatuhanGetOutgoingEdges` çağrıldığında, okuma esnasında verinin başka bir thread tarafından manipüle edilmesini önlemek amacıyla listenin doğrudan kendisi yerine bir kopyası (`new List<BatuhanTransactionEdge>(edges)`) döndürülür.
    

#### 2. Barındırdıkları Algoritmalar ve Analitik Kapsam

-   **DirectedGraph:** Grafın temel operasyonlarını (BFS, DFS, İleri ve Geri Fon Akışı) gerçekleştiren yalın graf modelidir. Temel düğüm ekleme ve bakiye sorgulama işlevlerinin doğruluğuna odaklanır.
    
-   **BlockchainGraph:** Projenin operasyonel analitik zekasını barındıran gelişmiş graf motorudur. `DirectedGraph` işlevlerine ek olarak şu kritik algoritmaları bünyesinde barındırır:
    
    -   **MehmetFindPath:** Belirtilen başlangıç adresinden hedef adrese giden en kısa transfer rotasını BFS ve parent-mapping yöntemiyle hesaplar.
        
    -   **MehmetFindMaxCapacityPath:** Finansal darboğaz analizi yapan, Dijkstra tabanlı bir Maksimum Kapasite Yolu algoritmasıdır. İki adres arasındaki alternatif yollardan akabilecek en yüksek hacimli transfer yolunu tespit eder.
        


Bu iki sınıfın aynı dizinde (`BlockchainAnalysis.DataStructures`) yer almasının ve projenizde birlikte bulunmasının mimari ve akademik gerekçeleri şunlardır:
-   **Arayüz Ayrımı ve Tek Sorumluluk (SOLID):** `DirectedGraph`, graf veri yapısının temel operasyonlarını (düğüm/kenar ekleme) en yalın haliyle yöneten bağımsız bir veri yapısıdır. `BlockchainGraph` ise bu yapıyı blokzincir simülasyonuna özgü kilit mekanizmaları ve karmaşık yol bulma algoritmalarıyla genişletir. İkisini birleştirmek sınıfın sorumluluğunu gereksiz yere artırır.
    
-   **Maliyet ve Performans Farkı:** `DirectedGraph`, sadece ilgili cüzdan nesnelerini (`BalanceLock`) kilitleyerek hafif ve daha az maliyetli bir eşzamanlılık sunar. `BlockchainGraph` ise tüm yapıyı tek bir merkezi kilit (`_graphLock`) altında toplar ve her okumada listenin kopyasını döner. Temel işlemlerde merkezi kilit maliyetine katlanmak istemediğiniz senaryolar için `DirectedGraph` elzemdir.
    
-   **Polimorfizm ve Esneklik Testleri:** İki sınıfın da `IGraph` arayüzünü implemente etmesi, `Program.cs` veya test katmanında tek bir satır değiştirerek farklı eşzamanlılık stratejilerinin performansını (kilitlenme maliyetlerini, işlem hızlarını) akademik olarak karşılaştırmanıza olanak tanır.

Geliştirdiğiniz projede bu iki graf yapısının kullanıldığı yerler şu şekildedir:

-   **DirectedGraph**, projenizin **arka uç (C#) test süreçlerinde** ve mikro düzeyde senkronizasyon gerektiren yalın veri yapısı doğrulama aşamalarında kullanılır. Özellikle `Program.cs` içindeki yorum satırına alınmış olan Faz 1 demo alanında; cüzdanların sisteme eklenmesi, temel transfer ilişkilerinin kurulması ve kilit maliyetlerinin en aza indirilerek hafif düzeyde (fine-grained) bakiye güncellemelerinin simüle edilmesi aşamalarında bu yapıdan yararlanılır.
    
-   **BlockchainGraph** ise projenizin **ana analitik motoru** olarak hem `Program.cs` üzerindeki **Faz 2 aktif test senaryolarında** hem de **FundFlowTracker** servisinde doğrudan kullanılır. **Kara Para Aklama ve Dolandırıcılık Tespitinde (AML)** (paranın izini süren döngü korumalı BFS algoritmaları `BatuhanGetForwardFundFlow` ve `BatuhanGetBackwardFundFlow`, iki cüzdan arasındaki en kısa transfer yolu aranırken (`MehmetFindPath`) ya da en yüksek hacimli ana finansal damar tespit edilmek istenirken (`MehmetFindMaxCapacityPath`) tamamen bu gelişmiş ve thread-safe sınıfın algoritmaları koşturulur.

## 6. Simülasyon ve Test Motoru (SyntheticDataGenerator.cs)

Bu sınıf, blokzincir analiz projenizin algoritmalarını (BFS, DFS, rota bulma vb.) test edebilmek için gerçeğe yakın, ancak yapay (sentetik) cüzdan ve işlem (transfer) verileri üreten bir **simülasyon motoru**dur. Gerçek bir veritabanına bağlanmaya gerek kalmadan, sistemin belleğinde test edilebilir bir ağ oluşturur.

**Temel Özellikleri ve Çalışma Mantığı**

-   Tekil Nesne (Singleton) Tasarımı: Sınıf, iş parçacığı güvenli (thread-safe) bir Singleton deseniyle yazılmıştır. Uygulama boyunca bellekte sadece bir örneği bulunur ve tüm servisler aynı test verisi havuzundan beslenir.
    
-   Sabit Rastgelelik (Fixed Seed): new Random(42) tanımı kullanılarak rastgelelik sabitlenmiştir. Bu sayede programı her çalıştırdığınızda tamamen aynı cüzdan adresleri ve aynı işlem tutarları üretilir. Bu özellik, algoritmik hataları ayıklamayı (debugging) tutarlı hale getirir.
    
-   O(1) Hızında Erişim Desteği: Üretilen veriler sadece standart listelere atılmaz; anında erişim sağlayabilmek için kendi yazdığınız HashTable yapılarına (DJB2 hash fonksiyonu kullanılarak) kopyalanır ve indekslenir.
    

**Kurguladığı Blokzincir Test Senaryoları**

Sınıf, graf veri yapınızı ve analiz motorlarınızı zorlamak için dört farklı tipolojik senaryo üretir:

-   Rastgele İşlemler (Random Part): Sistemdeki cüzdanlar arasında günlük, karmaşık ve düzensiz para transferlerini simüle eder.
    
-   Zincirleme Akış (Chain Flow): Paranın bir cüzdandan diğerine tek bir hat üzerinde sırayla aktarıldığı (A -> B -> C -> D) senaryodur. İleriye dönük fon izleme (Forward Flow) algoritmalarını test etmek için kullanılır.
    
-   Borsa Senaryosu (Exchange): Çok sayıda farklı cüzdandan, BINANCE_EXCHANGE isimli tek bir merkeze doğru gerçekleşen yoğun fon girişlerini modeller. Graf üzerindeki merkezileşmeyi ve maksimum kapasite yollarını test etmeye yarar.
    
-   Döngüsel İşlemler (Cycle): Paranın A'dan B'ye, B'den C'ye ve C'den tekrar A'ya döndüğü kapalı devre (A -> B -> C -> A) işlemleridir. Dolaşım algoritmalarınızın sonsuz döngüye girip girmediğini test eden en kritik senaryodur.
