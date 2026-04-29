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
        public WalletNode From { get; private set; }
        //paranın çıktığı kaynak cüzdan, private olmasıyla birlikte sadece okunabilir olması sağlanır
        public WalletNode To { get; private set; }
        //hedefteki cüzdandır.
        public double Amount { get; private set; }
        //graflarda kenarların üzerinde bir weight değeri bulunabilir. 
        //bizim modelimizde bu weight transfer edilen para miktarıdır.
        public DateTime Timestamp { get; private set; }
        //işlemin gerçekleştiği zaman bilgisi. blokzincirlerde kronoloji kritiktir.

        public TransactionEdge(WalletNode fromNode, WalletNode toNode, double amount)
        {//constructor. bi kenarın oluşabilmesi için yönünün ve ağırlığının verilmesini zorunlu kılar.
            if (fromNode == null || toNode == null)
            {//dangling edge yani hiçbi düğüme bağlı olmayan bi kenar olmasını enggeler
                throw new ArgumentNullException("Gönderen (From) ve alıcı (To) düğümler boş olamaz.");
            }

            if (amount <= 0)
            {//weight pozitif olmalı
                throw new ArgumentException("Transfer miktarı sıfırdan büyük olmalıdır.", nameof(amount));
            }

            From = fromNode;
            To = toNode;
            Amount = amount;
            Timestamp = DateTime.Now; // İşlemin oluşturulduğu anı otomatik kaydeder.
        }

        public override string ToString()
        {
            return $"Transaction: [{From.Address} -> {To.Address}] Amount: {Amount}, Time: {Timestamp.ToString("HH:mm:ss")}";
            //dolaşım algoritmaları çalıştırılırken konsol ekranında fon akışını takip etmeyi kolaylaştırır.
        }
    }
}