namespace BlockchainAnalysis.Api.Dtos;

public class MerkleNodeDto
{
    public string Hash { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string State { get; set; } = "default";
    public MerkleNodeDto? Left { get; set; }
    public MerkleNodeDto? Right { get; set; }
}

public class MerkleTreeDto
{
    public string RootHash { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public MerkleNodeDto RootNode { get; set; } = new();
}
