using System;
using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IGraph
    {
        void AddVertex(WalletNode wallet);
        void AddEdge(TransactionEdge transaction);
        List<TransactionEdge> GetForwardFundFlow(string startAddress);
        List<TransactionEdge> GetBackwardFundFlow(string startAddress);
        
        // BalanceEngine'in ihtiyaç duyduğu iki kritik metot:
        IReadOnlyList<TransactionEdge> GetOutgoingEdges(string address);
        IReadOnlyList<TransactionEdge> GetIncomingEdges(string address); 
    }
}