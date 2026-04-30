using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures;

public class BlockchainGraph : IGraph
{
    // Eski GraphNode yerine doğrudan WalletNode kullanıyoruz
    private readonly HashTable<string, WalletNode> _nodes = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly HashTable<string, List<TransactionEdge>> _adjacencyList = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly List<string> _addresses = new();

    public void AddVertex(WalletNode wallet)
    {
        AddNodeIfMissing(wallet.Address);
    }

    public void AddEdge(TransactionEdge edge)
    {
        AddNodeIfMissing(edge.FromAddress);
        AddNodeIfMissing(edge.ToAddress);

        _adjacencyList[edge.FromAddress].Add(edge);

        // AddOutgoing ve AddIncoming fonksiyonları yerine doğrudan kilitli bakiye güncellemesi yapıyoruz
        lock (_nodes[edge.FromAddress].BalanceLock)
        {
            _nodes[edge.FromAddress].Balance -= edge.Amount;
        }

        lock (_nodes[edge.ToAddress].BalanceLock)
        {
            _nodes[edge.ToAddress].Balance += edge.Amount;
        }
    }

    public IReadOnlyList<string> GetAddresses() => _addresses;

    public WalletNode GetNode(string address)
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

        _nodes.Add(address, new WalletNode(address));
        _adjacencyList.Add(address, new List<TransactionEdge>());
        _addresses.Add(address);
    }
}
