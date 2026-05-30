using System;
using System.Linq;
using BlockchainAnalysis.Models;
using BlockchainAnalysis.Core; // IGraph arayüzü için gerekli olabilir

namespace BlockchainAnalysis.Core
{
    public class UmmetDynamicBalanceEngine
    {
        private readonly IGraph _graph;

        // Dependency Injection ile graf nesnesi alınıyor
        public UmmetDynamicBalanceEngine(IGraph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        // Bakiye güncellemelerini eşzamanlı (Thread-safe) olarak gerçekleştirir.
        public void UmmetUpdateBalanceSafely(BatuhanWalletNode wallet, decimal amount, bool isIncoming)
        {
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

        // Belirtilen cüzdanın anlık bakiyesini, gelen ve giden transferleri hesaplayarak döndürür.
        public decimal UmmetCalculateDynamicBalance(string walletAddress)
        {
            var incomingEdges = _graph.BatuhanGetIncomingEdges(walletAddress);
            var outgoingEdges = _graph.BatuhanGetOutgoingEdges(walletAddress);

            // LINQ kullanılarak O(N) karmaşıklığında toplamlar hesaplanır
            decimal totalIncoming = incomingEdges.Sum(e => e.Amount);
            decimal totalOutgoing = outgoingEdges.Sum(e => e.Amount);

            return totalIncoming - totalOutgoing;
        }

        // Cüzdanın bakiye özetini konsola yazdırır.
        public void UmmetPrintWalletSummary(string walletAddress)
        {
            decimal dynamicBalance = UmmetCalculateDynamicBalance(walletAddress);
            Console.WriteLine($"[UmmetDynamicBalanceEngine] {walletAddress} - Anlık Hesaplanan Bakiye: {dynamicBalance:F2}");
        }
    }
}