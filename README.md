# ⛓️ Blokzincir Dolaşım Altyapısı: Stack & Queue
Bu repo, blokzincir ağındaki para transferlerini ve cüzdan hareketlerini izleyecek arama algoritmalarının çekirdek veri yapılarını içerir. Tüm mimari asenkron çalışmaya uygun şekilde sıfırdan kodlanmıştır.

## 🧱 Çekirdek Yapılar
* **`Node.cs`:** Ağdaki işlemleri (Transaction ID) birbirine bağlayan temel referans düğümü.
* **`Queue.cs` (Kuyruk):** Fon akışını katman katman izleyecek **BFS** algoritması için özel tasarlanmış FIFO yapısı.
* **`Stack.cs` (Yığıt):** Belirli bir cüzdanın geçmişine derinlemesine inmek için kullanılacak **DFS** algoritması için özel tasarlanmış LIFO yapısı.

## 🧠 Teknik Odak ve Performans
1. **Thread-Safe (Asenkron Güvenlik):** Yapay zeka simülasyonları veya çoklu veri akışı aynı anda çalıştığında sistemin çökmemesi için tüm işlemler `lock` mekanizmasıyla kilitlenmiştir.
2. **$O(1)$ Karmaşıklık:** Ekleme ve çıkarma (Push/Pop/Enqueue/Dequeue) işlemleri, döngü veya dizi kaydırması olmadan anında gerçekleşecek şekilde optimize edilmiştir.
3. **Saf C# Uygulaması:** Hazır `System.Collections` kütüphaneleri kullanılmadan, Faz-1 kurallarına uygun olarak saf pointer/referans mantığıyla geliştirilmiştir.