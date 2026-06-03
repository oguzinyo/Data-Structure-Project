# Proje Görev Dağılımı ve Teslimat Tablosu

| Faz | Sorumlu Kişi | Görev / Rol Tanımı | Ana Teslimat ve Sorumluluk |
| :--- | :--- | :--- | :--- |
| Faz 1 | Batuhan Özdemir | Yönlü Graf (Directed Graph) İskeleti | Graf iskeletini oluşturmak, düğüm (Node) ve kenar (Edge) sınıflarını basit nesne referanslarıyla birbirine bağlamak. |
| Faz 1 | Ali Kadir Özyaşar | Merkle Ağacı (Hash Tree) | İşlem verilerini ikili ağaç mantığıyla bağlamak, özyinelemeli hash hesaplamak ve veri bütünlüğü doğrulama algoritması kurmak. |
| Faz 1 | Oğuzhan Hekimoğlu | Karma Tablo (Hash Table) | O(1) erişim süresi için manuel hash fonksiyonu tasarlamak ve çakışma (collision) yönetimi geliştirmek. |
| Faz 1 | Ümmet Erkan | Kuyruk ve Yığıt (Queue & Stack) | Standart yığıt (push/pop) ve kuyruk (enqueue/dequeue) sınıflarını giriş seviyesinde kodlamak. |
| Faz 1 | M. Oğuzhan Tanrıverdi | Mimari ve DevOps | Docker konfigürasyonlarını, GitHub PR ayarlarını ve proje README dosyasını hazırlamak. |
| Faz 2 | Batuhan Özdemir | Fon Akışı İzleme ve Çekirdek Algoritma | Belirli bir cüzdan adresi girildiğinde fon akış geçmişini geriye/ileriye dönük kronolojik listeleyen filtreleme metotları. |
| Faz 2 | M. Oğuzhan Tanrıverdi | Hedef Düğüm Analisti ve Alternatif Yol | Hedef adres analizi metotları ve graf üzerinde alternatif yol bulma (Maksimum Kapasite Yolu) fonksiyonları. |
| Faz 2 | Ümmet Erkan | Dinamik Bakiye Motoru ve Eşzamanlılık | Thread-safe bakiye hesaplama, kilit mekanizmaları (lock) ve dinamik güncelleme motoru. |
| Faz 2 | Ali Kadir Özyaşar | Sentetik Veri Üreticisi ve Doğrulama | Sentetik veri üretim scriptleri, test girdi paketleri, GenAI destekli test senaryoları ve doğrulama çıktıları. |
| Faz 2 | Oğuzhan Hekimoğlu | Algoritmik Analizör ve Dokümantasyon | Zaman ve uzay karmaşıklığı (Big-O) analiz tabloları, UML şemaları ve kapsamlı Faz 2 ara raporu. |
| Faz 3 | Batuhan Özdemir | Merkle Ağacı Görselleştirme ve Panel | Seçilen bir işlem için Merkle ağacı yapısını tüm hiyerarşik aşamalarıyla ayrı panelde gösteren ve doğrulama durumunu renklendiren arayüz. |
| Faz 3 | Ali Kadir Özyaşar | Graf Görselleştirme ve Grafik Motoru | Yönlü okları net görünen, bakiye/miktar değişimlerine göre anlık şekil/boyut değiştiren esnek grafik paneli (D3.js/Cytoscape vb.). |
| Faz 3 | M. Oğuzhan Tanrıverdi | Etkileşim, Filtreleme ve Dinamik Vurgu | Arama paneli kontrolcüleri, fon akışı animasyonlu iz takip sistemi ve miktar/zaman bazlı dinamik grafik filtreleme bileşenleri. |
| Faz 3 | Oğuzhan Hekimoğlu | Backend API Entegrasyonu ve Veri Akışı | Ön yüzün tüm dinamik veri ihtiyacını karşılayan, in-memory veri yapılarını tüketen, hata yönetimi yapılmış RESTful API katmanı. |
| Faz 3 | Ümmet Erkan | DevOps, Docker Konfigüratörü ve Kalite | Frontend, Backend ve AI servislerinin izole ağlarda çalıştığı docker-compose yapısı, kararlılık testleri ve proje demo videosu. |


**Blokzincir İşlem Ağları Analizi (Blockchain Analysis)**

Bu proje, blokzincir üzerindeki işlem (transaction) ağlarını, cüzdan bakiyelerini ve veri akışlarını analiz etmek için geliştirilmiş tam yığın (full-stack) bir web uygulamasıdır. Arka planda .NET 10.0 Web API, ön yüzde ise Angular 19 kullanılmaktadır. Tüm sistem Docker üzerinden izole bir şekilde çalışacak biçimde yapılandırılmıştır.

**1. Sistem Gereksinimleri ve Ön Kurulumlar**

Bilgisayarınız yepyeni ise, projeyi sorunsuz çalıştırabilmek için öncelikle altyapı gereksinimlerini kurmanız şarttır.

* **Git Kurulumu:** Proje dosyalarını GitHub üzerinden indirmek için git-scm.com adresinden Git'i indirip standart ayarlarla kurun.
* **WSL 2 Altyapısı:** Docker'ın Windows üzerinde çalışabilmesi için Linux çekirdeğine ihtiyacı vardır. Yönetici olarak bir PowerShell açıp aşağıdaki komutu çalıştırın ve işlem bitince bilgisayarınızı yeniden başlatın:

```powershell
wsl --install

```

* **Docker Desktop Kurulumu:** [docker.com/products/docker-desktop](https://www.google.com/search?q=https://docker.com/products/docker-desktop) adresinden Docker Desktop'ı indirin. Kurulum ekranında "Use WSL 2 instead of Hyper-V" seçeneğinin işaretli olduğundan mutlaka emin olun. Kurulum bittiğinde programı başlatın ve sol alt köşedeki motor simgesinin yeşil (Engine is running) olmasını bekleyin. İlk açılışta çıkan lisans sözleşmelerini kabul edin.

**2. Projeyi Bilgisayara İndirme**

* Bilgisayarınızda projeyi saklamak istediğiniz klasöre gidin.
* O dizinde boş bir yere sağ tıklayıp "Open in Terminal" (Terminalde Aç) seçeneğine tıklayın.
* Aşağıdaki komut ile projeyi bilgisayarınıza klonlayın (Proje linkini kendi deponuza göre güncelleyebilirsiniz):

```bash
git clone https://github.com/oguzinyo/data-structure-project.git

```

* İndirme işlemi bitince terminal üzerinden projenin ana klasörüne geçiş yapın:

```bash
cd Data-Structure-Project/BlockchainAnalysis.Api

```

**3. Sistemi Ayağa Kaldırma**

Docker Desktop'ın arka planda açık ve çalışır durumda olduğundan emin olun. Proje dizininde açık olan terminalinize şu komutu yazarak sistemi sıfırdan derleyin ve başlatın:

```bash
docker compose up --build

```

Bu işlem bilgisayarınızın hızına ve internet bağlantınıza bağlı olarak birkaç dakika sürebilir. Terminal ekranında Angular'ın ve .NET'in çalıştığını belirten loglar (örneğin "Node Express server listening on http://localhost:4000") akmaya başladığında sistem hazır demektir. **Önemli:** Bu terminal penceresini çarpıdan kapatmayın, açık kaldığı sürece projeniz yayında kalır.

**4. Uygulamayı Kullanma**

Sistem çalışır duruma geldikten sonra herhangi bir web tarayıcısını açarak uygulamaya erişebilirsiniz.

* **Kullanıcı Arayüzü (Frontend):** Projenin görsel arayüzüne ve graflara ulaşmak için tarayıcınızın adres çubuğuna **http://localhost:4000** yazın.
* **API Dokümantasyonu (Backend):** Arka planda çalışan servislerin uç noktalarını (endpoint) görmek veya test etmek isterseniz **http://localhost:5050/scalar/v1** adresine gidebilirsiniz.

**5. Sistemi Durdurma**

Projeyle işiniz bittiğinde sunucuları kapatmak için logların aktığı terminal ekranına tıklayıp klavyenizden **Ctrl + C** tuşlarına basın.

Konteynerleri ve ağ yapılandırmalarını sistemden tamamen temizlemek isterseniz aynı proje dizininde şu komutu çalıştırabilirsiniz:

```bash
docker compose down

```
Proje Raporu Analizi->[Proje Raporu ve Analiz](https://github.com/user-attachments/files/28553262/Proje.Raporu.ve.Analiz.pdf)


