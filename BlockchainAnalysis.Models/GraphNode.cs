namespace BlockchainAnalysis.Models;

public class GraphNode
{
    public string Address { get; }
    public decimal IncomingTotal { get; private set; }
    public decimal OutgoingTotal { get; private set; }
    public decimal NetFlow => IncomingTotal - OutgoingTotal;

    public GraphNode(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Wallet address cannot be empty.", nameof(address));
        }

        Address = address;
    }

    public void AddIncoming(decimal amount)
    {
        IncomingTotal += amount;
    }

    public void AddOutgoing(decimal amount)
    {
        OutgoingTotal += amount;
    }
}
