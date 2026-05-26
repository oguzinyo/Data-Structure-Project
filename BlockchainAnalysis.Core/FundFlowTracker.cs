using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainAnalysis.Models;
using BlockchainAnalysis.DataStructures;

namespace BlockchainAnalysis.Core
{
    // Bu sınıf, Batuhan'ın "Fon Akışı İzleme" sorumluluğu kapsamında
    // ham graf verilerini filtrelemek ve analiz etmek için oluşturulmuştur.
    public class FundFlowTracker
    {
        private readonly BlockchainGraph _graph;

        public FundFlowTracker(BlockchainGraph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        // İleriye Dönük (Paranın nereye gittiği) - Filtreleme ve Katmanlı Takip
        public List<TransactionEdge> TrackForwardFlow(string startAddress, DateTime? startTime = null, DateTime? endTime = null, decimal? minAmount = null)
        {
            // Çekirdek algoritmadan tüm ileri yönlü akışı al (BlockchainGraph içindeki BFS)
            var allFlow = _graph.GetForwardFundFlow(startAddress);

            // Filtreleri uygula ve kronolojik olarak döndür
            return ApplyFilters(allFlow, startTime, endTime, minAmount);
        }

        // Geriye Dönük (Paranın nereden geldiği) - Filtreleme ve Kaynak İzleme
        public List<TransactionEdge> TrackBackwardFlow(string endAddress, DateTime? startTime = null, DateTime? endTime = null, decimal? minAmount = null)
        {
            // Çekirdek algoritmadan tüm geçmiş akışı al (BlockchainGraph içindeki BFS)
            var allPastFlow = _graph.GetBackwardFundFlow(endAddress);

            // Filtreleri uygula ve kronolojik olarak döndür
            return ApplyFilters(allPastFlow, startTime, endTime, minAmount);
        }

        // Yardımcı Filtreleme Metodu (Kod tekrarını önlemek için)
        private List<TransactionEdge> ApplyFilters(List<TransactionEdge> flow, DateTime? startTime, DateTime? endTime, decimal? minAmount)
        {
            var filteredFlow = flow.AsEnumerable();

            // 1. Zamana Göre Filtreleme (Sadece şu tarihten sonraki işlemler)
            if (startTime.HasValue)
            {
                filteredFlow = filteredFlow.Where(t => t.Timestamp >= startTime.Value);
            }

            // 2. Zamana Göre Filtreleme (Sadece şu tarihe kadar olan işlemler)
            if (endTime.HasValue)
            {
                filteredFlow = filteredFlow.Where(t => t.Timestamp <= endTime.Value);
            }

            // 3. Miktara Göre Filtreleme (Sadece şu bakiye limitinin üzerindeki transferler)
            if (minAmount.HasValue)
            {
                filteredFlow = filteredFlow.Where(t => t.Amount >= minAmount.Value);
            }

            // Blokzincir analizinin doğası gereği işlemleri zaman damgasına göre eskiden yeniye sırala
            return filteredFlow.OrderBy(t => t.Timestamp).ToList();
        }
    }
}