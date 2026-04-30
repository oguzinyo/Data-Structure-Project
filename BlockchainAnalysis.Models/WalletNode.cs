using System;
using System.Collections.Generic;
using System.Text;


namespace BlockchainAnalysis.Models
{
    public class Wallet
    {
        public string Address { get; set; }
        public decimal ApproximateBalance { get; set; }

        public Wallet(string address)
        {
            Address = address;
            ApproximateBalance = 0; // Başlangıçta bakiye 0
        }

    }

    
}
