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
        // Eski modele uyumluluk için eklenen özellikler
        public string TransactionId { get; private set; }
        public decimal Fee { get; private set; }

        // Dışarıdan string adres isteyen sınıflar (MerkleTree vb.) için uyumluluk köprüleri
        public string FromAddress => From?.Address ?? string.Empty;
        public string ToAddress => To?.Address ?? string.Empty;

        public WalletNode From { get; private set; }
        //paranın çıktığı kaynak cüzdan, private olmasıyla birlikte sadece okunabilir olması sağlanır
        
        public WalletNode To { get; private set; }
        //hedefteki cüzdandır.
        
        public decimal Amount { get; private set; }
        //graflarda kenarların üzerinde bir weight değeri bulunabilir. 
        //bizim modelimizde bu weight transfer edilen para miktarıdır.
        
        public DateTime Timestamp { get; private set; }
        //işlemin gerçekleştiği zaman bilgisi. blokzincirlerde kronoloji kritiktir.

        public TransactionEdge(WalletNode fromNode, WalletNode toNode, decimal amount, decimal fee = 0m)
        {//constructor. bi kenarın oluşabilmesi için yönünün ve ağırlığının verilmesini zorunlu kılar.
            if (fromNode == null || toNode == null)
            {//dangling edge yani hiçbi düğüme bağlı olmayan bi kenar olmasını enggeler
                throw new ArgumentNullException("Gönderen (From) ve alıcı (To) düğümler boş olamaz.");
            }

            if (amount <= 0)
            {//weight pozitif olmalı
                throw new ArgumentException("Transfer miktarı sıfırdan büyük olmalıdır.", nameof(amount));
            }

            if (fee < 0)
            {
                throw new ArgumentException("Madenci ücreti (Fee) negatif olamaz.", nameof(fee));
            }

            TransactionId = Guid.NewGuid().ToString(); // Benzersiz ID oluşturulur
            From = fromNode;
            To = toNode;
            Amount = amount;
            Fee = fee;
            Timestamp = DateTime.UtcNow; // İşlemin oluşturulduğu anı otomatik kaydeder.
        }

        public override string ToString()
        {
            return $"Transaction [{TransactionId}]: [{FromAddress} -> {ToAddress}] Amount: {Amount}, Fee: {Fee}, Time: {Timestamp.ToString("HH:mm:ss")}";
            //dolaşım algoritmaları çalıştırılırken konsol ekranında fon akışını takip etmeyi kolaylaştırır.
        }
    }
}
