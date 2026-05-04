Merkle Tree Verification System
Bu modül, Blokzincir İşlem Ağlarının Analizi projesi kapsamında, veri bütünlüğünü (integrity) sağlamak ve büyük veri setleri içindeki işlemleri hızlıca doğrulamak amacıyla geliştirilmiştir.

🛠 Merkle Tree Nasıl Çalışır?

Merkle Tree (Hash Tree), her yaprak düğümün bir veri bloğunun (bu projede blokzincir işlemleri) hash'ini temsil ettiği, yaprak olmayan her düğümün ise alt düğümlerinin hash'lerinin toplamının hash'ini temsil ettiği bir veri yapısıdır.

Yaprak Düğümleri (Leaf Nodes): Her bir işlem (transaction) SHA-256 gibi bir algoritma ile hash'lenir.

Eşleştirme: Hash'ler çiftler halinde gruplanır. Eğer tek sayıda işlem varsa, son işlem kendisiyle eşlenerek ağaç tamamlanır.

Hiyerarşik Özet: Çiftlenen hash'ler birleştirilip tekrar hash'lenir (Parent Hash). Bu işlem, tepede tek bir Merkle Root kalana kadar devam eder.

Doğrulama (Verification): Verideki tek bir bit değişse bile, bu değişim ağaç boyunca yukarı doğru yansır ve Merkle Root tamamen değişir. Bu sayede tüm veriyi kontrol etmeden verinin bozulup bozulmadığı anlaşılabilir.

⏳ Zaman Karmaşıklığı Analizi (Time Complexity)

Merkle Tree, özellikle büyük veri setlerinde (Big Data) verimliliği artırmak için tasarlanmıştır. İşlemlerin sayısı n olarak kabul edildiğinde karmaşıklık değerleri şu şekildedir:

Ağaç Oluşturma (Tree Construction): Tüm yaprakları hash'lemek ve yukarı doğru Root'a ulaşmak için her düğümü bir kez ziyaret eder. Karmaşıklık: O(n).

Merkle Root Hesaplama: Hiyerarşik yapı sayesinde işlem sayısı logaritmik olarak azalır.

Doğrulama (Verification / Merkle Proof): En büyük avantajı buradadır. Bir işlemin ağaçta olup olmadığını kanıtlamak için tüm ağacı taramak yerine sadece ağacın yüksekliği kadar hash kontrolü yapılır. Karmaşıklık: O(logn).

Veri Güncelleme: Bir yaprak değiştiğinde sadece o yaprağın bağlı olduğu dal (path) üzerindeki hash'ler güncellenir. Karmaşıklık: O(logn).

Özellikler:

Veri Bütünlüğü: İşlemlerin hiyerarşik bir doğrulama yapısı ile korunması.

C# Implementation: Nesne yönelimli programlama (OOP) prensiplerine uygun, optimize edilmiş Merkle Tree sınıfı.

Hızlı Sorgulama: Merkle Proof mantığı ile verimli veri doğrulama altyapısı.

Proje Yapısı:

Modül, projenin BlockchainAnalysis.DataStructures klasörü altında yer almaktadır:

MerkleTree.cs: Ağacın oluşturulması, hash hesaplama ve Merkle Root üretim mantığını içeren ana sınıf.

MerkleTree.csproj: Modülün bağımlılıklarını yöneten proje dosyası.

