using System;

/*
 * Sınıfın Amacı: Yönlü grafı oluşturan iki bileşen vardır.
 * Düğümler ve Kenarlar.
 * Projemizde cüzdan adreslerini düğümler temsil edecektir.
 * */

namespace BlockchainAnalysis.Models
{
    public class WalletNode
    {
        public string Address { get; private set; } //cüzdanın benzersiz kimliğidir.
        //okuma kısmı herkese açıktır fakat yazma sadece constructor içinde belirlenecektir, dışardan değiştirilemez (immutable)
        
        public decimal Balance { get; set; } //cüzdanın anlık bakiyesini tutacak
        //graf üzerinde transfer işlemleri gerçekleştikçe bakiye dinamik değişeceğinden get ve set public bırakıldı. 
        
        // Eski Wallet sınıfına uyumluluk (Geriye dönük destek için eklendi)
        public decimal ApproximateBalance 
        { 
            get => Balance; 
            set => Balance = value; 
        }

        public readonly object BalanceLock = new object();
        // Bakiye güncellemelerini senkronize etmek için kullanılacak kilit nesnesi
        public void AddFunds(decimal amount)
        {
            lock (BalanceLock)
            {
                Balance += amount;
            }
        }

        public void DeductFunds(decimal amount)
        {
            lock (BalanceLock)
            {
                Balance -= amount;
            }
        }

        public decimal GetCurrentBalance()
        {
            lock (BalanceLock)
            {
                return Balance;
            }
        }
        
        public WalletNode(string address) //constructor
        {
            if (string.IsNullOrWhiteSpace(address)) //dışardan gelen adres içeriye almaya uygun mu?
            {
                throw new ArgumentException("Cüzdan adresi boş olamaz.", nameof(address)); //hata fırlatır
            }

            Address = address;
            Balance = 0m; //yeni oluşturulan cüzdanın bakiyesi (decimal tip atama hatası 0.0 yerine 0m ile çözüldü)
        }

        public override string ToString() //override ile Object sınıfından gelen varsayılan metne çevrilme davranışını eziyoruz.
        {
            return $"Wallet [Address: {Address}, Balance: {Balance}]";
            /*
             * bununla doğrudan cüzdanın içeriğini okunabilir bir formatta görebileceğiz. 
             * bu durum ileride konsol çıktılarında işimizi kolaylaştıracaktır.
             */
        }
    }
}
