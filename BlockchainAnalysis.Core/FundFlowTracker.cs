using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainAnalysis.Models;
using BlockchainAnalysis.Core;

namespace BlockchainAnalysis.Core
{
    public class FundFlowTracker
    {
        private readonly IGraph _graph;

        // Şirket standartlarında Dependency Injection (Bağımlılık Enjeksiyonu) kullanımı
        public FundFlowTracker(IGraph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        public List<BatuhanTransactionEdge> BatuhanTrackForwardFlow(string startAddress, DateTime? startTime = null, DateTime? endTime = null, decimal? minAmount = null)
        {
            var allFlow = _graph.BatuhanGetForwardFundFlow(startAddress);
            return BatuhanApplyFilters(allFlow, startTime, endTime, minAmount);
        }

        public List<BatuhanTransactionEdge> BatuhanTrackBackwardFlow(string endAddress, DateTime? startTime = null, DateTime? endTime = null, decimal? minAmount = null)
        {
            var allPastFlow = _graph.BatuhanGetBackwardFundFlow(endAddress);
            return BatuhanApplyFilters(allPastFlow, startTime, endTime, minAmount);
        }

        private List<BatuhanTransactionEdge> BatuhanApplyFilters(List<BatuhanTransactionEdge> flow, DateTime? startTime, DateTime? endTime, decimal? minAmount)
        {
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
        }
    }
}