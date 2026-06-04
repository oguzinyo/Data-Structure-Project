using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainAnalysis.Models;
using BlockchainAnalysis.Core;

namespace BlockchainAnalysis.Core
{
    public class FundFlowTracker
    /*Bu sınıf, projenin çekirdek iş mantığını (Business Logic) temsil eden BlockchainAnalysis.Core katmanında yer alır. 
    Blokzincir ağı üzerindeki para transferlerinin izini sürmek, siber güvenlik senaryolarını simüle etmek ve graf motorundan dönen ham verileri iş mantığı kurallarına göre filtrelemek (budamak) için tasarlanmış merkezi bir servis konumundadır.*/
    {
        private readonly IGraph _graph;

        // Şirket standartlarında Dependency Injection (Bağımlılık Enjeksiyonu) kullanımı
        /*Bu mühendislik yaklaşımı (Dependency Injection), sistemin DirectedGraph veya BlockchainGraph gibi farklı graf motorlarıyla kod değiştirilmeden çalışabilmesini sağlar. 
        null referans kontrolü yapılarak (throw new ArgumentNullException) sistemin geçersiz bir motorla başlatılması engellenmiştir.*/
        public FundFlowTracker(IGraph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        public List<TransactionEdge> BatuhanTrackForwardFlow(string startAddress, DateTime? startTime = null, DateTime? endTime = null, decimal? minAmount = null)
        {
            /*Temel işlevi, belirtilen başlangıç cüzdanından çıkan fonların zaman içinde hangi adreslere ve hangi yollarla dağıldığını tespit etmektir.*/
            /*IGraph arayüzündeki BatuhanGetForwardFundFlow metodunu tetikleyerek graf üzerinden tüm alt dalları (raw flow) toplar ve ardından bu veriyi filtreleme motoruna gönderir.*/
            var allFlow = _graph.BatuhanGetForwardFundFlow(startAddress);
            return BatuhanApplyFilters(allFlow, startTime, endTime, minAmount);
        }

        public List<TransactionEdge> BatuhanTrackBackwardFlow(string endAddress, DateTime? startTime = null, DateTime? endTime = null, decimal? minAmount = null)
        {
            /*Hedef cüzdana ulaşan paranın (örneğin bir borsaya veya şüpheli bir hesaba giren paranın) kök kaynağını, yani ağa ilk giriş noktasını bulmayı hedefler.*/
            /*Bu algoritma, kara para aklama (AML) tespitlerinde ve siber güvenlik incelemelerinde hayati öneme sahiptir. İşleyiş mantığı olarak hedeften kaynağa doğru (reverse traversal) çalışır.*/
            var allPastFlow = _graph.BatuhanGetBackwardFundFlow(endAddress);
            return BatuhanApplyFilters(allPastFlow, startTime, endTime, minAmount);
        }

        private List<TransactionEdge> BatuhanApplyFilters(List<TransactionEdge> flow, DateTime? startTime, DateTime? endTime, decimal? minAmount)
        {
            /*Graf teorisinde ve bilgisayar bilimlerinde "budama", bir ağaç veya graf yapısı üzerinde arama yaparken, analizin amacına hizmet etmeyen, ilgisiz veya gereksiz olan dalların (kenarların/düğümlerin) arama uzayından kesilip atılması işlemidir.*/
            /*Bu metot, FundFlowTracker sınıfının içinde yer alan ve graf dolaşım algoritmalarından (BFS/DFS) dönen ham (raw) fon akışı verilerini budayarak temizleyen özel (private) bir yardımcı fonksiyondur.*/
            //Metot üç temel parametre üzerinden budama filtresi uygular:
                //Başlangıç Zamanı (startTime): İşlemin zaman damgası bu değerden eskiyse işlem budanır.
                //Bitiş Zamanı (endTime): İşlemin zaman damgası bu değerden yeniyse işlem budanır.
                //Minimum Tutar (minAmount): Transfer edilen miktar (Amount), analistin belirlediği alt sınırdan küçükse (örneğin 10 BTC altı işlemler) bu kenar budanarak listeden çıkarılır.
            var filteredFlow = flow.AsEnumerable();

            if (startTime.HasValue)
            {
                filteredFlow = filteredFlow.Where(t => t.Timestamp >= startTime.Value);
            }

            if (endTime.HasValue)
            {
                filteredFlow = filteredFlow.Where(t => t.Timestamp <= endTime.Value);
            }

            if (minAmount.HasValue)
            {
                filteredFlow = filteredFlow.Where(t => t.Amount >= minAmount.Value);
            }

            return filteredFlow.OrderBy(t => t.Timestamp).ToList();
            /*Budama işlemi bittikten sonra, geriye kalan işlemlerin analiz edilebilmesi için .OrderBy(t => t.Timestamp) komutu kullanılır. Bu komut, işlemleri her zaman kronolojik (gerçekleşme zamanı) sıraya dizerek olayların akış sırasını korur.*/
        }
    }
}
