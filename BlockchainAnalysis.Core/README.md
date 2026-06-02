
**BlockchainAnalysis.Core Dizini ve İş Mantığı Katmanı Analizi**

BlockchainAnalysis.Core dizini, projenizin merkezi iş mantığını (business logic) ve servislerini barındıran çekirdek katmandır. Bu katman, somut veri yapılarından (DataStructures dizinindeki sınıflardan) bağımsız çalışacak şekilde tasarlanmış olup, sistemin genel kurallarını ve hesaplama algoritmalarını soyutlamalar üzerinden yürütmektedir.

**1. Proje Yapılandırması: BlockchainAnalysis.Core.csproj**

-   **Ne İşe Yarar:** Core katmanının derleme ayarlarını ve bağımlılıklarını belirler.
    
-   **Nerede Kullanılır:** Projenin derlenmesi ve diğer katmanlar (App, DataStructures) tarafından referans alınması aşamasında kullanılır.
    
-   **Nasıl Çalışır:** Sadece BlockchainAnalysis.Models projesine bağımlıdır. Bu yapı, katmanlı mimarilerdeki Bağımlılıkların Tersine Çevrilmesi (Dependency Inversion) prensibine tam uyum sağlar. İş mantığı, veri yapılarını doğrudan bilmez, sadece veri modellerini bilir.
    

**2. Soyutlama ve Arayüzler (Interfaces)**

**IGraph.cs**

-   **Ne İşe Yarar:** Sistemdeki yönlü graf yapısının dış dünyaya (diğer servislere) sunduğu sözleşmeyi (contract) tanımlar.
    
-   **Nerede Kullanılır:** Bakiye hesaplama (DynamicBalanceEngine) ve fon akışı izleme (FundFlowTracker) sınıflarına enjekte edilerek (Dependency Injection) kullanılır.
    
-   **Nasıl Çalışır:** Düğüm ve kenar ekleme, gelen/giden kenarları listeleme ve ileri/geri fon akışı rotalarını getirme gibi graf teorisine dayalı temel fonksiyonların imzalarını barındırır. Bu sayede Core katmanı, grafın bellekte nasıl tutulduğuyla (örneğin Adjacency List mi, Matrix mi olduğuyla) ilgilenmez.
    

**IHashTable.cs**

-   **Ne İşe Yarar:** Cüzdan adreslerine O(1) karmaşıklıkta erişim sağlamak için kullanılacak hash tablosunun şablonunu belirler.
    
-   **Nerede Kullanılır:** Hızlı cüzdan araması gerektiren arka uç operasyonlarında kullanılır.
    
-   **Nasıl Çalışır:** Araya girme (Insert) ve geri getirme (Get) metotlarını tanımlayarak, alt katmanlarda yazılacak özel hash fonksiyonlarının (örneğin FNV-1a) bu standart üzerinden çağrılmasını güvence altına alır.
    

**3. Analiz ve Hesaplama Motorları**

**DynamicBalanceEngine.cs**

-   **Ne İşe Yarar:** Proje gereksinimleri belgesinde belirtilen Bakiye Hesaplama (Gelen transferlerin toplamı eksi Giden transferlerin toplamı) ve eşzamanlılık (Thread-safety) ihtiyaçlarını karşılar.
    
-   **Nerede Kullanılır:** Düğüm (cüzdan) bakiyelerinin anlık olarak güncellenmesi veya mevcut durumlarının sorgulanması gereken her işlem anında devreye girer.
    
-   **Nasıl Çalışır:** Dependency Injection ile aldığı IGraph arayüzü üzerinden ilgili cüzdanın tüm işlemlerini çeker. UmmetCalculateDynamicBalance metodu, LINQ (Language-Integrated Query) kullanarak O(N) zaman karmaşıklığında anlık bakiye hesaplar (N: İlgili cüzdana ait işlem sayısı). En kritik nokta olan UmmetUpdateBalanceSafely metodu ise asenkron ve çoklu iş parçacıklı (multi-threaded) ortamlarda veri bütünlüğünü korumak için cüzdan objesi üzerindeki BalanceLock nesnesini kilitleyerek (lock mekanizması) Race Condition (Yarış Durumu) hatalarını engeller.
    

**FundFlowTracker.cs**

-   **Ne İşe Yarar:** Blokzincir ağındaki belirli bir cüzdandan başlayan veya belirli bir cüzdana ulaşan paranın izini sürmek (graf üzerinde yol bulmak) ve bu veriyi filtrelemek için kullanılır.
    
-   **Nerede Kullanılır:** Siber güvenlik senaryolarında, kara para aklama analizlerinde veya App katmanındaki Senaryo A, B, C testlerinde kullanılır.
    
-   **Nasıl Çalışır:** Graf üzerinden ham BFS/DFS dolaşım sonuçlarını (Tüm akış) alır. BatuhanApplyFilters yardımcı metodu ile, gelen ham rotalar üzerinde zaman aralığı (startTime, endTime) ve minimum miktar (minAmount) budamaları (pruning) yapar. Bu işlem, istenmeyen graf dallarının filtrelenerek analizin spesifik bir hedefe daraltılmasını sağlar.
    

**Genel Değerlendirme**

Core katmanı, projenin gereksinim belgesinde (PDF) istenen "Thread-safe çalışma" ve "Fon Akış İzlemesi" gibi temel mühendislik şartlarını, sıkı bir nesne yönelimli tasarım (SOLID prensipleri) ile kodlamıştır. Arayüzler üzerinden çalışılması, ileride veri yapıları değişse bile bu çekirdek algoritmaların tek satır kod değiştirilmeden çalışmaya devam etmesini garanti etmektedir.
