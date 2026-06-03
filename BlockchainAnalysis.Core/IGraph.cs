using System;
using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IGraph
    {
        void BatuhanAddVertex(BatuhanWalletNode wallet);
        void BatuhanAddEdge(BatuhanTransactionEdge transaction);
        List<BatuhanTransactionEdge> BatuhanGetForwardFundFlow(string startAddress);
        List<BatuhanTransactionEdge> BatuhanGetBackwardFundFlow(string startAddress);
        
        // BalanceEngine'in ihtiyaç duyduğu iki kritik metot:
        IReadOnlyList<BatuhanTransactionEdge> BatuhanGetOutgoingEdges(string address);
        IReadOnlyList<BatuhanTransactionEdge> BatuhanGetIncomingEdges(string address); 
    }
}