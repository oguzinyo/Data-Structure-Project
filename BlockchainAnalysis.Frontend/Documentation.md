
**BlockchainAnalysis.Frontend Mimari Analizi**

Bu analiz, blokzincir analiz aracının ön yüz (Frontend) katmanının mevcut son durumunu temel almaktadır. Sistem, Angular 17+ Standalone Component mimarisi ve D3.js kullanılarak veri görselleştirme odaklı bir yapıda inşa edilmiştir.

**1. Genel Mimari ve Tasarım Yaklaşımı**

-   **Standalone Mimarisi:** Proje, geleneksel NgModule yapısından arındırılarak doğrudan bileşen tabanlı (component-driven) bir başlatma (bootstrap) sürecine geçirilmiştir. Bu durum, bileşenlerin bağımlılıklarını kendi içlerinde yönetmesini ve modülerliği artırır.
    
-   **Tasarım Dili (Glassmorphism):** Uygulama arayüzü, siber güvenlik temasına uygun olarak koyu arka plan üzerinde yarı saydam paneller ve neon vurgular (mor ve turkuaz) ile tasarlanmıştır. Panellerin z-index ve pointer-events yönetimi, arka plandaki grafiğin etkileşimini bozmayacak şekilde yapılandırılmıştır.
    

**2. App Bileşeni (Merkezi Orkestrasyon)**

-   **Durum (State) Yönetimi:** Tüm sistemin ana taşıyıcısıdır. Sol paneldeki filtreleme kontrolleri ile sağ paneldeki detay gösterimlerini ve arka plandaki graf motorunu koordine eder.
    
-   **Olay Yakalama (Event Handling):** Grafikten fırlatılan düğüm (cüzdan) ve kenar (işlem) tıklama olaylarını (BatuhanOnNodeClicked, BatuhanOnEdgeClicked) yakalayarak veri servisine asenkron istekler gönderir.
    
-   **Asenkron Senkronizasyonu:** Gelen asenkron verilerin arayüzde gecikme yaratmaması veya donmalara sebep olmaması için Angular'ın ChangeDetectorRef (cdr) sınıfı ile DOM güncellemeleri manuel olarak tetiklenmektedir.
    

**3. Graph Engine Bileşeni (D3.js Görselleştirme)**

-   **Fizik Simülasyonu:** Blokzincir ağı, d3.forceSimulation ile modellenmiştir. Düğümler birbirini iterken (charge), merkeze doğru bir çekim kuvveti uygulanır.
    
-   **Dinamik Veri Ölçeklendirme:** Düğüm çapları cüzdanın bakiyesine, işlem çizgilerinin kalınlığı ise transfer miktarına göre normalize edilerek (nRadius ve eWidth fonksiyonları) dinamik olarak çizilir.
    
-   **Çift Yönlü Transfer Yönetimi:** İki cüzdan arasında karşılıklı transfer olduğunda, çizgilerin üst üste binip veriyi gizlememesi için trigonometrik hesaplamalarla kavisli yollar (Bezier Curve) oluşturulur.
    
-   **Bellek Sızıntısı (Memory Leak) Optimizasyonu:** Arayüzdeki hacim filtresi kaydırıldığında veya grafik güncellendiğinde, işlemciyi kilitleyen yetim süreçleri (orphaned processes) engellemek amacıyla; her yeni çizimden önce eski fizik motoru örneği (this.simulation.stop() metodu ile) kesin olarak durdurulacak şekilde optimize edilmiştir.
    

**4. Merkle Panel Bileşeni (Kriptografik Ağaç)**

-   **Özyineli (Recursive) Şablon Mimarisi:** İşlem ağacının derinliği önceden bilinemeyeceği için statik HTML kullanılmamış; Angular'ın ng-template yapısı ile dallar bitene kadar kendini tekrar çağıran dinamik bir DOM inşa sistemi kurulmuştur.
    
-   **Kapsamlı Etkileşim:** Çok sayıda işlem barındıran büyük blokların incelenebilmesi için tarayıcının yerleşik Tam Ekran (Fullscreen) API'si entegre edilmiş ve CSS transform scale ile manuel yakınlaştırma/uzaklaştırma kontrolleri eklenmiştir.
    
-   **Yarış Durumu (Race Condition) Koruması:** Klavye girdileriyle veya hızlı tıklamalarla tetiklenen asenkron kriptografik (SHA-256) hesaplamaların birbirini ezmesini ve ekranda titreme yaratmasını engellemek için, işlem kimliği (execution token) veya bekleme (debounce) tabanlı senkronizasyon kontrolleriyle veri bütünlüğü güvence altına alınmıştır.
    

**5. Blockchain Data Service (Veri Sağlayıcı)**

-   **Asenkron Simülasyon:** Arka uç (Backend) entegrasyonu tamamlanana kadar, RxJS operatörleri (of, delay) kullanılarak API ağ gecikmeleri simüle edilmektedir.
    
-   **Algoritmik Merkle İnşası:** BatuhanGetMerkleTreeData metodu, kendisine gelen spesifik bir işlem kimliğini havuzdaki diğer işlemlerle logaritmik olarak birleştirir. Havuz boyutunu ikinin kuvvetlerine (padding) tamamlayarak yapraklardan tepeye doğru Root Hash değerini algoritmik olarak hesaplar ve ağacın durumunu (state: proof, computed, target) belirleyerek bileşene teslim eder.
