using System;
using System.Linq;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public class BalanceEngine
    {
        private readonly IGraph _graph;

        public BalanceEngine(IGraph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        public decimal CalculateDynamicBalance(string walletAddress)
        {
            // Sisteme gelen ve giden paraları çekiyoruz
            var incomingEdges = _graph.BatuhanGetIncomingEdges(walletAddress);
            var outgoingEdges = _graph.BatuhanGetOutgoingEdges(walletAddress);

            // Miktarları topluyoruz (Gelen - Giden kuralı)
            decimal totalIncoming = incomingEdges.Sum(e => e.Amount);
            decimal totalOutgoing = outgoingEdges.Sum(e => e.Amount);

            return totalIncoming - totalOutgoing;
        }
        
        public void PrintWalletSummary(string walletAddress)
        {
            decimal dynamicBalance = CalculateDynamicBalance(walletAddress);
            Console.WriteLine($"[Bakiye Motoru] {walletAddress} anlik hesaplanan bakiye: {dynamicBalance:F2}");
        }
    }
}