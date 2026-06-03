namespace BlockchainAnalysis.Api.Dtos;

public class FlowResultDto
{
    public string StartAddress { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public List<string> BfsOrder { get; set; } = new();
    public List<string> DfsOrder { get; set; } = new();
    public List<TransactionEdgeDto> FlowEdges { get; set; } = new();
}

public class PathResultDto
{
    public string StartAddress { get; set; } = string.Empty;
    public string TargetAddress { get; set; } = string.Empty;
    public List<string> Path { get; set; } = new();
    public bool Found { get; set; }
}

public class MaxCapacityPathResultDto
{
    public string StartAddress { get; set; } = string.Empty;
    public string TargetAddress { get; set; } = string.Empty;
    public List<string> Path { get; set; } = new();
    public bool Found { get; set; }
}

public class BalanceResultDto
{
    public string WalletAddress { get; set; } = string.Empty;
    public decimal DynamicBalance { get; set; }
}
