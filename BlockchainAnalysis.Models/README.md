
**BlockchainAnalysis.Models Katmanı Kapsamlı Mimari Analizi**

Bu dizin, projenin en alt seviyesini oluşturan, sistemin temel "varlıklarını" (entity) tanımlayan veri modelleri katmanıdır. Sistemdeki hiçbir katmana bağımlı değildir; aksine Core, DataStructures ve App katmanlarının tamamı, verileri taşımak ve işlemek için bu dizine bağımlıdır.

**5N1K Çerçevesinde Models Katmanı**

-   Ne: Blokzincir ağının yapıtaşları olan cüzdanları (düğümler) ve aralarındaki para transferlerini (kenarlar) nesne yönelimli olarak modelleyen sınıflar bütünüdür.
    
-   Nerede: Sistemin tüm katmanlarında (veri yapılarına eleman eklerken, graf üzerinde yol bulurken, sentetik veri üretirken ve bakiye hesaplarken) temel veri taşıyıcı model olarak kullanılır.
    
-   Ne Zaman: Uygulamanın çalışma anında (runtime), ağa yeni bir cüzdan eklendiğinde veya bir para transferi gerçekleştiğinde bellekte bu sınıflardan yeni nesneler türetilir.
    
-   Nasıl: Nesne Yönelimli Programlamanın kapsülleme kurallarına ve çoklu iş parçacığı güvenliği (thread-safety) prensiplerine uygun olarak tanımlanmış C# sınıfları aracılığıyla çalışır.
    
-   Neden: Proje gereksinimlerinde belirtilen "kripto işlem ağının yönlü bir graf olarak modellenmesi" şartını sağlamak için tasarlanmıştır. Grafın matematiksel soyutlamasını, bilgisayar belleğinde yönetilebilir somut nesnelere dönüştürmek için zorunlu bir yapıdır.
    
-   Kim: Projedeki tüm algoritmalar (BFS, DFS, Merkle Ağacı, Bakiye Motoru) bu veri modellerini tüketici sıfatıyla kullanarak işlem yapar.
    

**WalletNode Sınıfı (Graf Düğüm Mimarisi)**

Bu sınıf, yönlü graf üzerindeki cüzdanları (Vertex veya Node) temsil eder.

-   Temel Özellikler: Cüzdanın benzersiz kimliği olan Address özelliği dışarıdan değiştirilemez (immutable) yapıdadır ve sadece nesne oluşturulurken (constructor) belirlenir. Balance özelliği ise ağdaki transferler sonucunda graf üzerinde dinamik olarak değişebileceği için dışarıdan erişime açıktır.
    
-   Geriye Dönük Uyumluluk: Sistemdeki eski veya farklı isimlendirme kullanan kod bloklarının çökmemesi için ApproximateBalance isimli bir köprü özellik barındırır ve okuma/yazma işlemlerini doğrudan ana bakiyeye yönlendirir.
    
-   Eşzamanlılık ve Kilitleme (Thread-Safety): Proje PDF'indeki "Eşzamanlılık" şartını sağlamak adına readonly bir BalanceLock nesnesi içerir. AddFunds, DeductFunds ve GetCurrentBalance metotları çalışırken bu kilit (lock) devreye girer. Bu yapı, birden fazla analitik işlemin aynı anda bakiyeyi hatalı güncellemesini (Race Condition - Yarış Durumu) kesin olarak engeller.
    
-   Veri Güvenliği: Nesne oluşturulurken boş veya geçersiz bir cüzdan adresi girilmesi engellenmiş ve başlangıç bakiyesi, ondalık hassasiyet hatalarını önlemek için varsayılan olarak (0m) değerine eşitlenmiştir.
    

**TransactionEdge Sınıfı (Graf Kenar Mimarisi)**

Bu sınıf, yönlü graf üzerinde iki cüzdan düğümü arasında gerçekleşen para transferlerini (Edge veya Kenar) temsil eder.

-   Bileşenler: Paranın çıktığı kaynak cüzdan düğümü (From) ve hedefe ulaştığı cüzdan düğümü (To) nesnelerini referans olarak içinde barındırır. Graf teorisinde kenarın ağırlığını (weight) temsil eden Amount (transfer miktarı), ağ madenci ücreti olan Fee ve işlemin kronolojisini belirleyen Timestamp (zaman damgası) özelliklerine sahiptir.
    
-   Benzersiz Kimlik: Her transfer işlemine sistem tarafından kurucu metot içerisinde otomatik olarak bir TransactionId (GUID) atanır. Bu benzersiz özellik, blokzincirdeki döngüsel transferlerin analizinde, işlemin daha önce ziyaret edilip edilmediğini kontrol etmek ve sonsuz döngüleri engellemek için graf dolaşım algoritmalarında kritik bir rol oynar.
    
-   Uyumluluk Köprüleri: Merkle Ağacı ve karma tablo (Hash Table) algoritmalarında, nesne yerine string tabanlı adres sorgulamaları yapabilmek için FromAddress ve ToAddress gibi yardımcı erişim özellikleri eklenmiştir.
    
-   Korumalı İnşa (Constructor Validations): Hiçbir düğüme bağlı olmayan, havada asılı (dangling edge) hatalı işlemlerin oluşmasını engellemek için kurucu metot içinde From ve To nesnelerinin null olup olmadığı denetlenir. Aynı zamanda sıfırın altında bir transfer miktarı veya negatif komisyon ücreti girilmesi, ArgumentException hatası fırlatılarak engellenmiştir.
    

**Proje Yapılandırması (BlockchainAnalysis.Models.csproj)**

Bu dosya, Models katmanının bağımlılıklarını belirler. Dikkat çekici olan en önemli detay, herhangi bir dış kütüphaneye veya projedeki diğer alt/üst katmanlara hiçbir referans barındırmamasıdır. Yalnızca .NET 10.0 platformu ve temel güvenlik (Nullable referanslar) ayarlarını içerir. Bu mimari izolasyon, model sınıflarının sistemin her yerinde güvenle ve kopyalanabilir şekilde kullanılmasını sağlar.
