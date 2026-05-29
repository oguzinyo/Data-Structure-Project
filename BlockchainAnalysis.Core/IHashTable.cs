using System;
using System.Collections.Generic;
using System.Text;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public interface IHashTable
    {
        void Insert(string address, BatuhanWalletNode wallet);
        BatuhanWalletNode Get(string address);
    }
}
