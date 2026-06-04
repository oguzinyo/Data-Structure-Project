using System;

/*
 * Sınıfın Amacı: Yönlü grafı oluşturan iki bileşen vardır.
 * Düğümler ve Kenarlar.
 * Projemizde cüzdan adreslerini düğümler temsil edecektir.
 * Mimari Rolü: Çoklu iş parçacığı (multi-threading) ortamında bakiye tutarsızlıklarını (Race Condition) önlemek için kendi içinde bir kilitleme (BalanceLock) mekanizması barındırır.
 * */

namespace BlockchainAnalysis.Models
{
    public class WalletNode
    {
        public string Address { get; private set; } //cüzdanın benzersiz kimliğidir.
        //okuma kısmı herkese açıktır fakat yazma sadece constructor içinde belirlenecektir, dışardan değiştirilemez (immutable)
        
        public decimal Balance { get; set; } //cüzdanın anlık bakiyesini tutacak
        //graf üzerinde transfer işlemleri gerçekleştikçe bakiye dinamik değişeceğinden get ve set public bırakıldı. 
        
        // Sistemin eski veya farklı isimlendirme kullanan versiyonlarındaki kodların kırılmaması için geriye dönük uyumluluk amacıyla eklenmiş bir köprü özelliktir. 
        // Okuma ve yazma işlemlerini doğrudan ana Balance değişkenine yönlendirir.
        public decimal ApproximateBalance 
        { 
            get => Balance; 
            set => Balance = value; 
        }

        public readonly object BalanceLock = new object();
        // Bakiye güncellemelerini senkronize etmek için kullanılacak kilit nesnesi
        /*BalanceLock: Eşzamanlılık (concurrency) yönetimi için kullanılan salt okunur bir nesnedir. 
        Çoklu iş parçacıklı (multi-threading) senaryolarda aynı anda birden fazla işlemin aynı bakiyeyi değiştirmeye çalışması durumunda oluşacak yarış durumunu (race condition) önlemek için kilit mekanizmasında kullanılır.*/
        public void AddFunds(decimal amount)
        {
            /*AddFunds: Cüzdana dışarıdan para eklenmesini sağlar. 
            İşlem 'lock (BalanceLock)' bloğu içinde çalıştığı için, aynı anda birden fazla iş parçacığı bu cüzdana para eklemeye çalışırsa işlemler sıraya sokulur ve bakiye kayması engellenir.*/
            lock (BalanceLock)
            {
                Balance += amount;
            }
        }

        public void DeductFunds(decimal amount)
        {
            /*DeductFunds: Cüzdandan para çıkışı yapar. Yine kilit mekanizması ile iş parçacığı güvenliği (thread-safety) sağlanır.*/
            lock (BalanceLock)
            {
                Balance -= amount;
            }
        }

        public decimal GetCurrentBalance()
        {
            /*GetCurrentBalance: Cüzdanın mevcut bakiyesini okur. Okuma işlemi sırasında kilit mekanizması kullanılarak, o an arka planda güncellenmekte olan tamamlanmamış bir bakiye değerinin okunması engellenir.*/
            lock (BalanceLock)
            {
                return Balance;
            }
        }
        
        public WalletNode(string address) //constructor
        {
            /*WalletNode Constructor: Sınıfın kurucu metodudur. Yeni bir nesne oluşturulurken parametre olarak gelen cüzdan adresinin boş olup olmadığını kontrol eder. 
            Boş veya geçersiz ise bir 'ArgumentException' hatası fırlatarak çalışmayı durdurur. Geçerliyse adresi sisteme kaydeder ve başlangıç bakiyesini sıfır olarak (0m) belirler. 
            (Sondaki 'm' harfi, verinin ondalıklı finansal işlemler için en hassas tip olan decimal olduğunu belirtir).*/
            if (string.IsNullOrWhiteSpace(address)) //dışardan gelen adres içeriye almaya uygun mu?
            {
                throw new ArgumentException("Cüzdan adresi boş olamaz.", nameof(address)); //hata fırlatır
            }

            Address = address;
            Balance = 0m; //yeni oluşturulan cüzdanın bakiyesi (decimal tip atama hatası 0.0 yerine 0m ile çözüldü)
        }

        public override string ToString() //override ile Object sınıfından gelen varsayılan metne çevrilme davranışını eziyoruz.
        {
            /*ToString: Cüzdanın içeriğini konsol çıktılarında okunabilir bir metin formatında göstermek için temel 'Object' sınıfından miras alınan varsayılan metodu ezer (override). 
            Dolaşım algoritmaları loglanırken bu sayede doğrudan adres ve bakiye bilgisi görünür.*/
            return $"Wallet [Address: {Address}, Balance: {Balance}]";
            /*
             * bununla doğrudan cüzdanın içeriğini okunabilir bir formatta görebileceğiz. 
             * bu durum ileride konsol çıktılarında işimizi kolaylaştıracaktır.
             */
        }
    }
}
