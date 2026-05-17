using System;
using System.Collections.Generic;
using System.Text;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IHashTable
    {
        void Insert(string address, WalletNode wallet);
        WalletNode Get(string address);
    }
}
