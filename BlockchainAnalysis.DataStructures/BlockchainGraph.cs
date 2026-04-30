using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;

namespace BlockchainAnalysis.DataStructures;

public class BlockchainGraph : IGraph
{
    private readonly HashTable<string, GraphNode> _nodes = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly HashTable<string, List<TransactionEdge>> _adjacencyList = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly List<string> _addresses = new();

    public void AddVertex(Wallet wallet)
    {
        AddNodeIfMissing(wallet.Address);
    }

    public void AddEdge(Transaction transaction)
    {
        AddTransactionEdge(new TransactionEdge(transaction));
    }

    public void AddTransactionEdge(TransactionEdge edge)
    {
        AddNodeIfMissing(edge.FromAddress);
        AddNodeIfMissing(edge.ToAddress);

        _adjacencyList[edge.FromAddress].Add(edge);
        _nodes[edge.FromAddress].AddOutgoing(edge.Amount);
        _nodes[edge.ToAddress].AddIncoming(edge.Amount);
    }

    public IReadOnlyList<string> GetAddresses() => _addresses;

    public GraphNode GetNode(string address)
    {
        if (_nodes.TryGetValue(address, out var node))
        {
            return node;
        }

        throw new KeyNotFoundException($"Graph node not found: {address}");
    }

    public IReadOnlyList<TransactionEdge> GetOutgoingEdges(string address)
    {
        if (!_adjacencyList.TryGetValue(address, out var edges))
        {
            return Array.Empty<TransactionEdge>();
        }

        return edges;
    }

    public List<string> BreadthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var queue = new CustomQueue<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return order;
        }

        visited.Add(startAddress, true);
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var current = queue.Dequeue();
            order.Add(current);

            foreach (var edge in GetOutgoingEdges(current))
            {
                if (!visited.ContainsKey(edge.ToAddress))
                {
                    visited.Add(edge.ToAddress, true);
                    queue.Enqueue(edge.ToAddress);
                }
            }
        }

        return order;
    }

    public List<string> DepthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var stack = new CustomStack<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return order;
        }

        stack.Push(startAddress);

        while (!stack.IsEmpty)
        {
            var current = stack.Pop();

            if (visited.ContainsKey(current))
            {
                continue;
            }

            visited.Add(current, true);
            order.Add(current);

            var outgoingEdges = GetOutgoingEdges(current);
            for (int i = outgoingEdges.Count - 1; i >= 0; i--)
            {
                var nextAddress = outgoingEdges[i].ToAddress;
                if (!visited.ContainsKey(nextAddress))
                {
                    stack.Push(nextAddress);
                }
            }
        }

        return order;
    }

    private void AddNodeIfMissing(string address)
    {
        if (_nodes.ContainsKey(address))
        {
            return;
        }

        _nodes.Add(address, new GraphNode(address));
        _adjacencyList.Add(address, new List<TransactionEdge>());
        _addresses.Add(address);
    }
}
