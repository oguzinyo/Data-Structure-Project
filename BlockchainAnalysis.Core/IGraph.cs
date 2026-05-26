using System;
using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IGraph
    {
        void AddVertex(WalletNode wallet);
        void AddEdge(TransactionEdge transaction);
        IReadOnlyList<TransactionEdge> GetIncomingEdges(string address);
        List<TransactionEdge> GetForwardFundFlow(string startAddress);
        List<TransactionEdge> GetBackwardFundFlow(string startAddress);
    }
}