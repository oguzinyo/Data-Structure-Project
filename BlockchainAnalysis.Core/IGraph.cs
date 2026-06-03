using System;
using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IGraph
    {
        void BatuhanAddVertex(WalletNode wallet);
        void BatuhanAddEdge(TransactionEdge transaction);
        List<TransactionEdge> BatuhanGetForwardFundFlow(string startAddress);
        List<TransactionEdge> BatuhanGetBackwardFundFlow(string startAddress);
        
        // BalanceEngine'in ihtiyaç duyduğu iki kritik metot:
        IReadOnlyList<TransactionEdge> BatuhanGetOutgoingEdges(string address);
        IReadOnlyList<TransactionEdge> BatuhanGetIncomingEdges(string address); 
    }
}