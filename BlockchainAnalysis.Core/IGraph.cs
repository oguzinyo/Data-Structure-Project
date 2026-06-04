using System;
using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
/*
SOLID Prensipleri ve Bağımlılıkların Tersine Çevrilmesi
Yazılım mühendisliğindeki Bağımlılıkların Tersine Çevrilmesi (Dependency Inversion Principle) kuralının projenizdeki temel yansımasıdır. FundFlowTracker veya UmmetDynamicBalanceEngine gibi çekirdek servisler, 
verinin bellekte karma tabloyla (HashTable) mı yoksa iki boyutlu matrisle mi tutulduğuyla ilgilenmez. Sadece IGraph arayüzünü tanırlar ve bu arayüzün sunduğu metotları tüketirler. 
Bu tasarım sayesinde, gelecekte DirectedGraph sınıfını tamamen silip yerine farklı bir graf kütüphanesi veya veritabanı bağlamak isterseniz, çekirdek iş mantığı kodlarında tek bir satır bile değişiklik yapmanıza gerek kalmaz.
*/
    public interface IGraph
        /*IGraph arayüzü, sistemdeki tüm graf veri yapıları (DirectedGraph, BlockchainGraph) 
        için bağlayıcı bir sözleşme (contract) tanımlar. BlockchainAnalysis.Core katmanında yer alması tesadüf değildir; bu dosya, üst seviye iş mantığı sınıfları ile alt seviye somut veri yapıları arasındaki sınır çizgisidir.*/
    {
        void BatuhanAddVertex(WalletNode wallet);
        //BatuhanAddVertex: Grafa yeni bir WalletNode (cüzdan düğümü) eklenmesini zorunlu kılar.
        void BatuhanAddEdge(TransactionEdge transaction);
        //BatuhanAddEdge: Düğümler arasına yönlü bir TransactionEdge (işlem kenarı) eklenmesini zorunlu kılar.
        List<TransactionEdge> BatuhanGetForwardFundFlow(string startAddress);
        //BatuhanGetForwardFundFlow: Belirtilen adresten çıkan paranın ileriye dönük izini sürecek algoritmanın imzasını taşır. FundFlowTracker sınıfı, filtreleme yapmadan önce tüm dolaşım verisini çekmek için bu metodu çağırır.
        List<TransactionEdge> BatuhanGetBackwardFundFlow(string startAddress);
        //BatuhanGetBackwardFundFlow: Belirtilen adrese gelen paranın geriye dönük kök kaynağını bulacak algoritmanın imzasını taşır.
        
        // BalanceEngine'in ihtiyaç duyduğu iki kritik metot:
        IReadOnlyList<TransactionEdge> BatuhanGetOutgoingEdges(string address);
        //BatuhanGetOutgoingEdges: UmmetDynamicBalanceEngine sınıfının, bir cüzdandan çıkan toplam parayı hesaplayabilmesi için ihtiyaç duyduğu salt okunur giden işlemler listesini sağlar.
        IReadOnlyList<TransactionEdge> BatuhanGetIncomingEdges(string address); 
        //BatuhanGetIncomingEdges: Bakiye hesaplama motorunun, bir cüzdana giren toplam parayı hesaplayabilmesi için ihtiyaç duyduğu salt okunur gelen işlemler listesini sağlar.
    }
}
