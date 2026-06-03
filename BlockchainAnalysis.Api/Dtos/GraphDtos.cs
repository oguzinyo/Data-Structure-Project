namespace BlockchainAnalysis.Api.Dtos;

public class WalletNodeDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class TransactionEdgeDto
{
    public string Id { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public string Timestamp { get; set; } = string.Empty;
}

public class GraphDataDto
{
    public List<WalletNodeDto> Nodes { get; set; } = new();
    public List<TransactionEdgeDto> Edges { get; set; } = new();
}
