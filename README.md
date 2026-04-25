# Blokzincir İşlem Ağlarının Analizi

Bu proje, blokzincir sistemlerindeki işlem verilerini sadeleştirilmiş bir model üzerinden incelemeyi amaçlayan bir "Veri Yapıları" dönem projesidir. Sistemde cüzdanlar düğüm (vertex), para transferleri ise kenar (edge) olarak modellenmiştir.

## 🏗️ Proje Mimarisi ve Çekirdek Veri Yapıları
- **Yönlü Graf (Directed Graph):** Cüzdanlar arası para akışının modellenmesi.
- **Merkle Ağacı (Hash Tree):** İşlem verilerinin değiştirilip değiştirilmediğini denetleyen kriptografik doğrulama.
- **Karma Tablo (Hash Table):** Cüzdan ID'lerine O(1) karmaşıklığında anında erişim.
- **Max-Heap (Mempool):** İşlemlerin sisteme girmeden önce işlem ücretine (fee) göre bekletildiği öncelikli havuz.

## 🚀 Projeyi Ayağa Kaldırma (Docker)
Proje, tüm ortam bağımlılıklarından izole edilecek şekilde konteynerize edilmiştir. Sisteminizde Docker yüklü ise tek bir komutla projeyi derleyip çalıştırabilirsiniz:

```bash
docker-compose up --build


```markdown

## 👥 Takim Uyeleri
- Oguzhan HEKİMOGLU
- Ali Kadir OZYASAR
- Batuhan OZDEMİR
- Ummet ERKAN
- Mehmet Oguzhan TANRIVERDİ 