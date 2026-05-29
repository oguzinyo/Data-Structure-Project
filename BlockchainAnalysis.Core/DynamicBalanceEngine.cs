using System;
using System.Collections.Generic;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Core
{
    public class DynamicBalanceEngine
    {
        // Thread-safe bakiye güncelleme fonksiyonu
        public void UpdateBalanceSafely(WalletNode wallet, decimal amount, bool isIncoming)
        {
            // Cüzdan kilitleniyor, başka hiçbir işlem aynı anda bu bakiyeyi değiştiremez
            lock (wallet.BalanceLock)
            {
                if (isIncoming)
                {
                    wallet.Balance += amount;
                }
                else
                {
                    wallet.Balance -= amount;
                }
            }
        }

        // PDF Kuralı: Gelen transferlerin toplamı - Giden transferlerin toplamı
        public decimal CalculateCurrentBalanceSafely(string address, IReadOnlyList<TransactionEdge> incomingEdges, IReadOnlyList<TransactionEdge> outgoingEdges)
        {
            decimal totalIncoming = 0m;
            decimal totalOutgoing = 0m;

            // Gelen transferlerin toplamı hesaplanıyor
            foreach (var edge in incomingEdges)
            {
                totalIncoming += edge.Amount;
            }

            // Giden transferlerin toplamı hesaplanıyor
            foreach (var edge in outgoingEdges)
            {
                totalOutgoing += edge.Amount;
            }

            // Gelen - Giden net bakiye (Basitleştirilmiş model)
            return totalIncoming - totalOutgoing;
        }
    }
}