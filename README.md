Bu proje, bir blokzincir ağındaki işlem verilerinin bütünlüğünü ve doğruluğunu kontrol etmek amacıyla C# ile geliştirilmiş
bir Merkle Tree (Karma Ağacı) uygulamasıdır.



📌 Özellikler
Veri Bütünlüğü: Yaprak düğümlerdeki (leaf nodes) herhangi bir değişikliğin kök hash (Root Hash) üzerindeki etkisi gözlemlenebilir.

Hash Algoritması: Veriler, kriptografik olarak güvenli bir şekilde birleştirilerek ağaç yapısı oluşturulur.

Hızlı Doğrulama: Büyük veri setleri içinde belirli bir işlemin varlığı, tüm veri setini kontrol etmeye gerek kalmadan doğrulanabilir.



🛠 Teknik Detaylar
Dil: C# [.NET]

Mimari: İkili Ağaç (Binary Tree) yapısı üzerine kurgulanmıştır.

Dosyalar:

MerkleTreeFunc.cs: Ağacın oluşturulması ve hash hesaplama mantığını içeren temel sınıf.

Demo.cs: Sistemin nasıl çalıştığını gösteren test ve senaryo dosyası.