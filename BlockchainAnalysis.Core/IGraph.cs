using System;
using System.Collections.Generic;
using System.Text;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IGraph
    {
        void AddVertex(WalletNode wallet);
        void AddEdge(TransactionEdge transaction);
    }
}
