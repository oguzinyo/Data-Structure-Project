namespace BlockchainAnalysis.Models;

public class TransactionEdge
{
    public string TransactionId { get; }
    public string FromAddress { get; }
    public string ToAddress { get; }
    public decimal Amount { get; }
    public DateTime Timestamp { get; }

    public TransactionEdge(Transaction transaction)
    {
        TransactionId = transaction.TransactionId;
        FromAddress = transaction.FromAddress;
        ToAddress = transaction.ToAddress;
        Amount = transaction.Amount;
        Timestamp = transaction.Timestamp;
    }
}
