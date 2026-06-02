using BlockchainAnalysis.Api.Dtos;
using BlockchainAnalysis.Core;
using BlockchainAnalysis.DataStructures;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.Api.Services;

public class BlockchainService
{
    private readonly BlockchainGraph _graph;
    private readonly AliSyntheticDataGenerator _dataGenerator;
    private readonly FundFlowTracker _flowTracker;
    private readonly UmmetDynamicBalanceEngine _balanceEngine;
    private readonly List<BatuhanTransactionEdge> _allTransactions;

    public BlockchainService()
    {
        _graph = new BlockchainGraph();
        _dataGenerator = new AliSyntheticDataGenerator(20);

        foreach (var wallet in _dataGenerator.WalletsTable)
        {
            _graph.BatuhanAddVertex(wallet);
        }

        var transactions = _dataGenerator.TransactionsTable;

        var totalOutgoing = new Dictionary<string, decimal>();
        foreach (var tx in transactions)
        {
            if (!totalOutgoing.ContainsKey(tx.FromAddress))
                totalOutgoing[tx.FromAddress] = 0;
            totalOutgoing[tx.FromAddress] += tx.Amount;
        }

        foreach (var kvp in totalOutgoing)
        {
            var node = _graph.BatuhanGetNode(kvp.Key);
            node.AddFunds(kvp.Value);
        }

        foreach (var tx in transactions)
        {
            _graph.BatuhanAddEdge(tx);
        }

        _allTransactions = transactions;
        _flowTracker = new FundFlowTracker(_graph);
        _balanceEngine = new UmmetDynamicBalanceEngine(_graph);
    }

    public GraphDataDto GetGraphData()
    {
        var addresses = _graph.BatuhanGetAddresses();
        var nodes = new List<WalletNodeDto>();
        var edges = new List<TransactionEdgeDto>();

        foreach (var address in addresses)
        {
            var node = _graph.BatuhanGetNode(address);
            nodes.Add(new WalletNodeDto
            {
                Id = node.Address,
                Label = node.Address,
                Balance = node.Balance
            });
        }

        foreach (var address in addresses)
        {
            foreach (var edge in _graph.BatuhanGetOutgoingEdges(address))
            {
                edges.Add(MapEdgeToDto(edge));
            }
        }

        return new GraphDataDto { Nodes = nodes, Edges = edges };
    }

    public List<WalletNodeDto> GetAllNodes()
    {
        var addresses = _graph.BatuhanGetAddresses();
        var nodes = new List<WalletNodeDto>();

        foreach (var address in addresses)
        {
            var node = _graph.BatuhanGetNode(address);
            nodes.Add(new WalletNodeDto
            {
                Id = node.Address,
                Label = node.Address,
                Balance = node.Balance
            });
        }

        return nodes;
    }

    public List<TransactionEdgeDto> GetAllEdges()
    {
        var addresses = _graph.BatuhanGetAddresses();
        var edges = new List<TransactionEdgeDto>();

        foreach (var address in addresses)
        {
            foreach (var edge in _graph.BatuhanGetOutgoingEdges(address))
            {
                edges.Add(MapEdgeToDto(edge));
            }
        }

        return edges;
    }

    public FlowResultDto GetForwardFlow(string startAddress, decimal? minAmount = null)
    {
        var bfsOrder = _graph.BatuhanBreadthFirstTraversal(startAddress);
        var dfsOrder = _graph.BatuhanDepthFirstTraversal(startAddress);
        var flowEdges = _flowTracker.BatuhanTrackForwardFlow(startAddress, minAmount: minAmount);

        return new FlowResultDto
        {
            StartAddress = startAddress,
            Direction = "forward",
            BfsOrder = bfsOrder,
            DfsOrder = dfsOrder,
            FlowEdges = flowEdges.Select(MapEdgeToDto).ToList()
        };
    }

    public FlowResultDto GetBackwardFlow(string startAddress, decimal? minAmount = null)
    {
        var flowEdges = _flowTracker.BatuhanTrackBackwardFlow(startAddress, minAmount: minAmount);

        return new FlowResultDto
        {
            StartAddress = startAddress,
            Direction = "backward",
            BfsOrder = new List<string>(),
            DfsOrder = new List<string>(),
            FlowEdges = flowEdges.Select(MapEdgeToDto).ToList()
        };
    }

    public PathResultDto FindPath(string startAddress, string targetAddress)
    {
        var path = _graph.MehmetFindPath(startAddress, targetAddress);

        return new PathResultDto
        {
            StartAddress = startAddress,
            TargetAddress = targetAddress,
            Path = path,
            Found = path.Count > 0
        };
    }

    public MaxCapacityPathResultDto FindMaxCapacityPath(string startAddress, string targetAddress)
    {
        var path = _graph.MehmetFindMaxCapacityPath(startAddress, targetAddress);

        return new MaxCapacityPathResultDto
        {
            StartAddress = startAddress,
            TargetAddress = targetAddress,
            Path = path,
            Found = path.Count > 0
        };
    }

    public BalanceResultDto GetDynamicBalance(string walletAddress)
    {
        var balance = _balanceEngine.UmmetCalculateDynamicBalance(walletAddress);

        return new BalanceResultDto
        {
            WalletAddress = walletAddress,
            DynamicBalance = balance
        };
    }

    public MerkleTreeDto GetMerkleTree(string? transactionId = null)
    {
        var payloads = new List<string>();
        var txIds = new List<string>();

        foreach (var tx in _allTransactions)
        {
            string payload = $"{tx.TransactionId}{tx.FromAddress}{tx.ToAddress}{tx.Amount}";
            payloads.Add(payload);
            txIds.Add(tx.TransactionId);
        }

        var merkleTree = new AliMerkleTree();
        string rootHash = merkleTree.Build(payloads);

        int targetIndex = -1;
        if (transactionId != null)
        {
            targetIndex = txIds.IndexOf(transactionId);
        }

        var rootNode = BuildMerkleNodeDto(merkleTree.Root, payloads, txIds, targetIndex);
        if (MarkMerkleProofPath(rootNode))
        {
            rootNode.State = "root";
        }

        return new MerkleTreeDto
        {
            RootHash = rootHash,
            IsValid = merkleTree.Verify(payloads, rootHash),
            RootNode = rootNode
        };
    }

    public List<string> GetTransactionIds()
    {
        return _allTransactions.Select(tx => tx.TransactionId).ToList();
    }

    private TransactionEdgeDto MapEdgeToDto(BatuhanTransactionEdge edge)
    {
        return new TransactionEdgeDto
        {
            Id = edge.TransactionId,
            Source = edge.FromAddress,
            Target = edge.ToAddress,
            Amount = edge.Amount,
            Fee = edge.Fee,
            Timestamp = edge.Timestamp.ToString("o")
        };
    }

    private static MerkleNodeDto BuildMerkleNodeDto(
        AliMerkleNode? node, List<string> payloads, List<string> txIds, int targetIndex, int leafIndex = 0)
    {
        if (node == null)
            return new MerkleNodeDto { Hash = "", State = "default" };

        var dto = new MerkleNodeDto { Hash = node.Hash, State = "default" };

        if (node.Left == null && node.Right == null)
        {
            if (leafIndex < txIds.Count)
                dto.Label = txIds[leafIndex];

            if (leafIndex == targetIndex)
                dto.State = "target";
        }
        else
        {
            int leftLeafCount = CountLeaves(node.Left);
            dto.Left = BuildMerkleNodeDto(node.Left, payloads, txIds, targetIndex, leafIndex);
            dto.Right = BuildMerkleNodeDto(node.Right, payloads, txIds, targetIndex, leafIndex + leftLeafCount);
        }

        return dto;
    }

    private static int CountLeaves(AliMerkleNode? node)
    {
        if (node == null) return 0;
        if (node.Left == null && node.Right == null) return 1;
        return CountLeaves(node.Left) + CountLeaves(node.Right);
    }

    private static bool MarkMerkleProofPath(MerkleNodeDto node)
    {
        if (node.Left == null && node.Right == null)
            return node.State == "target";

        bool leftHasTarget = node.Left != null && MarkMerkleProofPath(node.Left);
        bool rightHasTarget = node.Right != null && MarkMerkleProofPath(node.Right);

        if (leftHasTarget || rightHasTarget)
        {
            node.State = node.Left?.State == "root" || node.Right?.State == "root" ? "root" : "computed";

            if (leftHasTarget && node.Right != null && node.Right.State == "default")
                node.Right.State = "proof";
            if (rightHasTarget && node.Left != null && node.Left.State == "default")
                node.Left.State = "proof";

            return true;
        }

        return false;
    }
}
