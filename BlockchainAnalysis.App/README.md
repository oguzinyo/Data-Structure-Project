
# BlockchainAnalysis.App Klasörü Dokümantasyonu

## Klasörün Genel Amacı

BlockchainAnalysis.App klasörü, Blokzincir İşlem Ağları Analizi projesinin ana giriş noktasını temsil eden bir konsol uygulamasıdır. Bu katman, projede geliştirilen veri yapıları (DataStructures), temel iş mantığı (Core) ve veri modellerinin (Models) entegre bir şekilde çalışmasını test etmek ve sunmak amacıyla oluşturulmuştur. Projenin işleyişi ve test senaryoları bu uygulama üzerinden çalıştırılıp gözlemlenir.

## İçerisindeki Dosyalar ve İşlevleri

### 1. BlockchainAnalysis.App.csproj

Bu dosya, konsol uygulamasının proje yapılandırmasını ve bağımlılık ağacını tanımlar.

-   Uygulamanın bir konsol uygulaması (Executable) olarak derleneceğini yapılandırır.
    
-   Projenin .NET 10.0 altyapısında çalışmasını sağlayacak hedef çerçeve (TargetFramework) ayarlarını barındırır.
    
-   Uygulamanın çalışabilmesi için zorunlu olan BlockchainAnalysis.Core, BlockchainAnalysis.DataStructures ve BlockchainAnalysis.Models projelerine ait referans bağlantılarını kurar.
    

### 2. Program.cs

Projenin çalıştırılabilir kodlarını ve algoritmaların doğrulama senaryolarını barındıran ana kaynak dosyasıdır. İçerisinde veri yapılarının işlevselliğini kanıtlayan iki temel faz kurgulanmıştır:

-   **Faz 1 (Genel Veri Yapıları Demo):** Sentetik işlem verileri oluşturularak cüzdanların ve transferlerin bir graf yapısına (BlockchainGraph) işlenmesini sağlar. Hash tablolarının çakışma (collision) ve yük faktörü istatistikleri test edilir. Yönlü transfer ağında Queue ile BFS ve Stack ile DFS graf dolaşım yaklaşımları sergilenir. Ayrıca işlemlerin bütünlüğünü doğrulamak için Merkle Ağacı mekanizmasının çalışması, manipüle edilmiş veriler üzerinden simüle edilir.
    
-   **Faz 2 (Fon Akış Takibi - Fund Flow Tracker):** Sistemin ileri düzey filtreleme yeteneklerini gösteren test aşamasıdır. Test düğümleri (Alice, Bob, Charlie, David) ve aralarındaki transferler tanımlanarak ağ üzerine eklenir. Belirli bir cüzdandan çıkan tüm fon akışının izlenmesi (İleriye Dönük BFS), transfer miktarının belirli bir değerin (örneğin 100 birim) üzerinde olduğu durumların filtrelenmesi ve bir cüzdana gelen fonların geçmişinin taranması (Geriye Dönük BFS) gibi senaryolar çalıştırılır.
    

## Modüller Arası İlişki

Bu klasör kendi içerisinde yeni bir veri yapısı veya karmaşık bir algoritma inşa etmez. Görevi, diğer katmanlarda yazılan soyut ve karmaşık yapıları somutlaştırarak okunabilir terminal çıktıları üretmektir. Projenin genel mimarisinde, geliştirilen mühendislik çözümlerinin (graf, hash, filtreleme motorları) doğru ve stabil bir şekilde çalıştığını kanıtlayan vitrin modülü olarak konumlanmaktadır.