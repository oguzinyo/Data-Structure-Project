using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IGraph
    {
        void AddVertex(WalletNode wallet);
        void AddEdge(TransactionEdge transaction);

        // Faz 2: Fon akışı izleme için eklenen metot
        IReadOnlyList<TransactionEdge> GetIncomingEdges(string address);
    }
}