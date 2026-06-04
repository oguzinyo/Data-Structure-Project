using System;

/*
 Kripto işlem ağı yönlü bir graf olarak modellenmelidir. Bu sınıf yönlü grafın
 kenar yapısını (edge) temsil etmektedir.
 Cüzdan adresleri arası para transferleri, miktar ve zaman bilgisi taşıyan yönlü kenarlar
 olarak modelleneceklerdir. İşlemlerin bir göndericisi ve bir alıcısı
 olduğundan ötürü grafın kenarları da yönlü olacaktır.
 */

namespace BlockchainAnalysis.Models
{
    public class TransactionEdge
    {
        public string TransactionId { get; private set; }
     /*TransactionId: İşleme atanan benzersiz tanımlayıcıdır (GUID). "private set" ile korunarak dışarıdan manipüle edilmesi engellenmiştir. 
     Dolaşım algoritmaları (BFS/DFS) çalışırken, graf üzerindeki döngüsel transferlerin (sonsuz döngülerin) tespit edilip engellenmesinde anahtar rol oynar.*/
        public decimal Fee { get; private set; }
     /*Fee: İşlem için ödenen madenci (ağ) ücretidir.*/

        // Dışarıdan string adres isteyen sınıflar (MerkleTree vb.) için uyumluluk köprüleri
        public string FromAddress => From?.Address ?? string.Empty;
        public string ToAddress => To?.Address ?? string.Empty;
     /*FromAddress ve ToAddress: Merkle Ağacı gibi, doğrudan string formatında adres metnine ihtiyaç duyan diğer bileşenler için yazılmış uyumluluk köprüleridir. Eğer düğüm (From/To) null ise uygulamanın çökmemesi için boş string (string.Empty) döndürürler.*/

     /*From ve To: İşlemin gönderici ve alıcı uçlarını temsil eden 'WalletNode' referanslarıdır. Bu referanslar, kenarın yönlülük özelliğini (A'dan B'ye) belirler. Sadece okunabilir yapıdadırlar.*/
     
        public WalletNode From { get; private set; }
        //paranın çıktığı kaynak cüzdan, private olmasıyla birlikte sadece okunabilir olması sağlanır
        
        public WalletNode To { get; private set; }
        //hedefteki cüzdandır.

     /*Amount: Graf teorisindeki "kenar ağırlığına" (weight) karşılık gelir. İki düğüm arasında aktarılan finansal değeri (fon miktarını) ifade eder.*/
        public decimal Amount { get; private set; }
        //graflarda kenarların üzerinde bir weight değeri bulunabilir. 
        //bizim modelimizde bu weight transfer edilen para miktarıdır.

     /*Timestamp: İşlemin gerçekleştiği anı tutar. Blokzincir mimarilerinde işlemlerin kronolojik sıralaması bütünlük açısından zorunlu olduğundan bu veri hayati önem taşır.*/
        public DateTime Timestamp { get; private set; }
        //işlemin gerçekleştiği zaman bilgisi. blokzincirlerde kronoloji kritiktir.

        public TransactionEdge(WalletNode fromNode, WalletNode toNode, decimal amount, decimal fee = 0m)
        {//constructor. bi kenarın oluşabilmesi için yönünün ve ağırlığının verilmesini zorunlu kılar.
            if (fromNode == null || toNode == null)
            {
             /*Havada Asılı Kenar (Dangling Edge) Koruması: 
             Yeni bir işlem nesnesi oluşturulurken, gönderici (fromNode) veya alıcı (toNode) parametrelerinden birinin null olması durumunda ArgumentNullException fırlatılır. 
             Bu denetim, graf üzerinde hiçbir düğüme bağlı olmayan geçersiz kenarların oluşmasını kesin olarak engeller.*/
                throw new ArgumentNullException("Gönderen (From) ve alıcı (To) düğümler boş olamaz.");
            }
         
             /*Mantıksal Değer Koruması: Transfer miktarının (amount) sıfır veya negatif olması, madenci ücretinin (fee) ise negatif olması ArgumentException ile engellenir.*/
            if (amount <= 0)
            {
                throw new ArgumentException("Transfer miktarı sıfırdan büyük olmalıdır.", nameof(amount));
            }

            if (fee < 0)
            {
                throw new ArgumentException("Madenci ücreti (Fee) negatif olamaz.", nameof(fee));
            }

         /*Otomatik Üretimler: Tüm doğrulamalar geçildiğinde, sisteme dışarıdan verilmesine gerek kalmadan otomatik olarak benzersiz bir TransactionId (Guid.NewGuid) ve UTC saat dilimine ayarlı bir Timestamp (DateTime.UtcNow) üretilip atanır.*/
            TransactionId = Guid.NewGuid().ToString(); // Benzersiz ID oluşturulur
            From = fromNode;
            To = toNode;
            Amount = amount;
            Fee = fee;
            Timestamp = DateTime.UtcNow; // İşlemin oluşturulduğu anı otomatik kaydeder.
        }

        public override string ToString()
        {
         /*ToString: Object sınıfından gelen temel davranışı ezen (override) yardımcı bir fonksiyondur. 
         Karmaşık nesne referansını okunaklı bir metin dizisine çevirir. Özellikle BFS/DFS graf dolaşım algoritmaları konsolda çalıştırılırken, 
         paranın hangi adresten hangi adrese (A -> B), ne miktarda ve saat kaçta aktığını takip etmeyi tek satırda mümkün kılar.*/
            return $"Transaction [{TransactionId}]: [{FromAddress} -> {ToAddress}] Amount: {Amount}, Fee: {Fee}, Time: {Timestamp.ToString("HH:mm:ss")}";
            //dolaşım algoritmaları çalıştırılırken konsol ekranında fon akışını takip etmeyi kolaylaştırır.
        }
    }
}
