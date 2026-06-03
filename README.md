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
cd data-structure-project/Data-Structure-Project-integration-backend

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
