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

# 👥 Detaylı Contributor Analizi

## 1️⃣ **BatuhanOzdemir7** (52 commit)

### İnşa Ettiği Yapılar:
- **Veri Yapı Implementasyonları:**
  - Graph algoritmaları
  - Blockchain node ve edge yapıları

- **Core Engine & Backend:**
  - BlockchainGraph ana sınıfı ve metodları
  - Search ve traversal operasyonları
  - Entity-Relationship modeli

- **Veri Yönetimi:**
  - Frontend-Backend bağlantı mimarı
  - Data flow control
  - Caching mekanizmaları
  - State management
 
- **Docker:**
  - Dockerfile optimization - Multi-stage build setup
 
- **Documentation:**
  - README.md teknik içeriği
  - Her ana klasörde dökümantasyon
### Yaptığı İşler:
```
✓ Graph engine'in mimarı (BatuhanGraph engine)
✓ Blockchain bileşenleri (WalletNode, TransactionEdge modeling)
✓ Frontend-Backend entegrasyon mimarı
✓ Dataset loading ve processing pipeline
✓ Test framework'ün kurulması
✓ Core algorithm implementations
✓ Performance optimizations
✓ Dokumentasyon ve code comments
```
---

## 2️⃣ **TheQayu** (M.Oğuzhan Tanrıverdi) (12 commit)


### Geliştirdiği Özellikler:

#### 🎨 **UI/UX Tasarım:**
- **Glassmorphism mimarisi ve transparan arayüz bileşenleri**
  - Modern, transparan UI componentleri
  - Gradient arka planlar, blur efektleri ve modern renk paleti

- **Dashboard & Panels:**
  - Canlı veri akışını gösteren Activity Feed bileşeni
  - JSON visualization panelleri
  -Gerçek zamanlı (real-time) veri gösterim ekranları
  - Interactive UI elementleri

#### 🔗 **Blockchain Components:**
- **UI Interaction Mekanizmaları**
  - Node seçimi ve highlighting
  - Edge visualizasyonu
  - Transaction flow gösterimi

#### 📋 **Proje Infrastructure:**
- İlk proje skeleton'unu oluşturdu
- Folder structure ve organization
- Docker ve docker-compose dosyaları
- CI/CD pipeline setup

#### 📚 **Dokumentasyon:**
- README.md ilk versiyonu
- Takım üyeleri listesi
- Proje açıklaması ve setup instructions
- Commit message standardları

### Yaptığı İşler:
```
✓ Proje skeleton template'inin kurulması
✓ Glassmorphism UI framework tasarımı
✓ Activity Feed component geliştirmesi
✓ JSON data visualization paneli entegrasyonu
✓ MehmetUpdateFilter ile dinamik hacim (slider) filtreleme sistemi
✓ MehmetFindPath ile animasyonlu BFS rota vurgulama kontrolü
✓ Sayfa kaydırma yerleşim hatası çözümü (sticky status bar layout fix)
✓ Borsa cüzdanları için özel kırmızı renk stilizasyonu (exchange node styling)
✓ UI interaction ve tıklama event yöneticileri
✓ Docker infrastructure ve docker-compose kurulumu
✓ README teknik dökümantasyonu ve kurulum rehberi
✓ Merge commit yönetimi ve PR süreçlerinin takibi
```

---

## 3️⃣ **ummeter0** (Ümmet ERKAN) (9 commit)


### Geliştirdiği Sistemler:

#### ⚙️ **Core Engine:**
- **UmmetDynamicBalanceEngine**
  - Dinamik bakiye hesaplama algoritması
  - Real-time balance updates
  - Multi-threaded processing
  - Optimized calculations

- **Thread-Safety Implementation:**
  - Thread-safe data structures
  - Concurrent operations handling
  - Lock mechanisms
  - Race condition prevention
  - Synchronization patterns

#### 🐳 **DevOps & Infrastructure:**
- **Docker Microservices Architecture**
  - Backend container setup
  - Frontend container configuration
  - Database container orchestration
  - Network bridges ve volume management

- **Configuration Management:**
  - docker-compose.yml files
  - Environment variable management
  - Service dependencies
  - Port mappings ve networking

- **Dependency Resolution:**
  - NuGet packages management
  - Frontend library fixes
  - Version compatibility
  - Build optimization

#### 🔧 **Code Quality & Maintenance:**
- **IGraph Interface Completion**
  - Missing method implementations
  - Interface contract fulfillment
  - Method signatures standardization

- **Data Structures:**
  - Queue implementation
  - Stack implementation
  - Final testing ve validation

### Yaptığı İşler:
```
✓ UmmetDynamicBalanceEngine sınıfı
✓ Thread-safe core engine
✓ Docker microservices setup
✓ Docker-compose orchestration
✓ Frontend dependency fixing
✓ IGraph interface completion
✓ Queue and Stack implementation
✓ Naming standards uygulaması
✓ Performance optimization
✓ Multi-threading synchronization
```

---

## 4️⃣ **Alozysr** (Ali Kadir Özyaşar) (6 commit)

### Yönettiği Alanlar:

#### 📖 **Documentation Management:**
- **README.md Maintenance**
  - Format corrections
  - Link formatting iyileştirmeleri
  - Content clarity improvements
  - Markdown syntax optimization

- **Proje Raporları**
  - Report links eklemesi
  - Documentation structure
  - Reference management
  - Resource linking

#### 🔀 **Merge & Code Review:**
- **Conflict Resolution**
  - Merge conflict çözümleri
  - Branch merging
  - Code review operations
  - Integration testing

- **PR Management**
  - Feature/dataset-integration PR merge
  - Feature/graph-visualization PR merge
  - Pull request reviews
  - Code quality checks

#### ✅ **Quality Assurance:**
- **Testing & Validation**
  - Graph visualization testing
  - Dataset integration testing
  - Link verification
  - Format validation

### Yaptığı İşler:
```
✓ README.md conflict resolution
✓ Link formatting standardization
✓ Project report integration
✓ Feature/graph-visualization merge
✓ Feature/dataset-integration merge
✓ Documentation consistency
✓ Markdown formatting improvements
✓ PR review ve approval
✓ Quality assurance checks
✓ Release management preparation
```

---

## 5️⃣ **oguzinyo** (Oğuzhan Hekimoğlu) (6 commit)


### Yönettiği Süreçler:

#### 🔐 **Repository Management:**
- **Repository Configuration**
  - Branch policies
  - Merge rules
  - Access controls
  - Workflow setup

- **PR Coordination**
  - PR approvals
  - Merge operations
  - Release management
  - Version control

#### 💾 **Data Structure Implementation:**
- **Hash Table Implementation**
  - Hash function design
  - Collision handling (chaining/open addressing)
  - Insert, search, delete operations
  - Load factor management
  - Resize operations

- **Algorithm Design:**
  - Hash computation
  - Bucket management
  - Performance optimization
  - Edge case handling

#### 🔗 **Backend Integration:**
- **API Integration**
  - Backend service connections
  - Endpoint configuration
  - Data format standardization
  - Error handling

- **System Integration**
  - Component interconnection
  - Data flow management
  - Service orchestration
  - Interface compatibility

### Yaptığı İşler:
```
✓ Hash table data structure
✓ Hash function implementation
✓ Collision resolution strategy
✓ Backend integration architecture
✓ Backend service configuration
✓ API endpoint routing
✓ Data serialization
✓ PR merge approval (PR #15, #34, #35)
✓ Release coordination
✓ Repository maintenance
✓ Version control management
```
---

## 📈 Zaman Çizelgesi & Faz Analizi

### **Faz 1: Temel Altyapı (25 Nisan - 4 Mayıs)**
- **Sorumlu:** TheQayu, BatuhanOzdemir7
- **Çalışma:** Proje iskeletesi, Docker kurulumu, temel klasör yapısı

### **Faz 2: Veri Yapıları (4 Mayıs - 17 Mayıs)**
- **Sorumlu:** BatuhanOzdemir7, ummeter0, oguzinyo
- **Çalışma:** LinkedList, Stack, Queue, Hash Table implementasyonları

### **Faz 3: Core Engine (17 Mayıs - 29 Mayıs)**
- **Sorumlu:** BatuhanOzdemir7, ummeter0, TheQayu
- **Çalışma:** Blockchain Graph, Balance Engine, Thread-Safe yapılar

### **Faz 4: UI & Visualization (29 Mayıs - 30 Mayıs)**
- **Sorumlu:** TheQayu, BatuhanOzdemir7
- **Çalışma:** Glassmorphism UI, JSON panels, Activity Feed

### **Faz 5: Integration & Polish (30 Mayıs - 3 Haziran)**
- **Sorumlu:** ummeter0, oguzinyo, Alozysr
- **Çalışma:** Backend integration, Documentation, Dataset integration

---
## 🔧 **TEKNIK KATKILAR**

| Kategori | BatuhanOzdemir7 | TheQayu | ummeter0 | Alozysr | oguzinyo | TOPLAM |
|----------|---|---|---|---|---|---|
| **Backend/Core Engine** | ⭐⭐⭐⭐⭐ (65%) | ⭐ (5%) | ⭐⭐⭐⭐⭐ (25%) | - | ⭐⭐ (5%) | 100% |
| **Frontend/UI/UX** | ⭐⭐ (25%) | ⭐⭐⭐ (35%) | - | ⭐⭐⭐ (35%) | ⭐ (5%) | 100% |
| **Veri Yapıları** | ⭐⭐⭐⭐ (30%) | - | ⭐⭐⭐⭐ (30%) | ⭐⭐⭐⭐ (20%) | ⭐⭐⭐ (20%) | 100% |
| **DevOps/Docker** | ⭐⭐ (35%) | ⭐ (20%) | ⭐⭐⭐⭐⭐ (40%) | - | ⭐ (5%) | 100% |
| **API Layer** | ⭐⭐⭐⭐⭐ (70%) | - | ⭐ (10%) | - | ⭐⭐ (20%) | 100% |
| **Dokumentasyon** | ⭐⭐⭐⭐ (40%) | ⭐ (10%) | ⭐ (10%) | ⭐⭐ (15%) | ⭐⭐⭐ 30%) | 100% |
| **Proje Yönetimi** | ⭐⭐⭐ (45%) | ⭐ (5%) | ⭐ (5%) | ⭐⭐ (20%) | ⭐⭐⭐ (25%) | 100% |

---

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
Demo Videosu->[Demo Videosu](https://youtu.be/WYRro70tyJs)


