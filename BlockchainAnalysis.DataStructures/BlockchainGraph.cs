using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures;

public class BlockchainGraph : IGraph
{
    // Eski GraphNode yerine doğrudan BatuhanWalletNode kullanıyoruz
    private readonly HashTable<string, BatuhanWalletNode> _nodes = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly HashTable<string, List<BatuhanTransactionEdge>> _adjacencyList = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly List<string> _addresses = new();
    private readonly object _graphLock = new object();

    public void BatuhanAddVertex(BatuhanWalletNode wallet)
    {
        lock (_graphLock)
        {
            BatuhanAddNodeIfMissing(wallet.Address);
        }
    }

public void BatuhanAddEdge(BatuhanTransactionEdge edge)
    {
        lock (_graphLock)
        {
            BatuhanAddNodeIfMissing(edge.FromAddress);
            BatuhanAddNodeIfMissing(edge.ToAddress);

            _adjacencyList[edge.FromAddress].Add(edge);
        }

        // Kendi yazdığın thread-safe metotlarla bakiyeleri güncelliyoruz
        _nodes[edge.FromAddress].DeductFunds(edge.Amount);
        _nodes[edge.ToAddress].AddFunds(edge.Amount);
    }

    public IReadOnlyList<string> BatuhanGetAddresses() => _addresses;

    public BatuhanWalletNode BatuhanGetNode(string address)
    {
        if (_nodes.TryGetValue(address, out var node))
        {
            return node;
        }

        throw new KeyNotFoundException($"Graph node not found: {address}");
    }

public IReadOnlyList<BatuhanTransactionEdge> BatuhanGetOutgoingEdges(string address)
    {
        lock (_graphLock)
        {
            if (!_adjacencyList.TryGetValue(address, out var edges))
            {
                return Array.Empty<BatuhanTransactionEdge>();
            }
            
            // Okuma sırasında veri değişmesin diye listenin kopyasını döndürüyoruz
            return new List<BatuhanTransactionEdge>(edges);
        }
    }

    public List<string> BatuhanBreadthFirstTraversal(string startAddress)
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

            foreach (var edge in BatuhanGetOutgoingEdges(current))
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

    public List<string> BatuhanDepthFirstTraversal(string startAddress)
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

            var outgoingEdges = BatuhanGetOutgoingEdges(current);
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

    private void BatuhanAddNodeIfMissing(string address)
    {
        if (_nodes.ContainsKey(address))
        {
            return;
        }

        _nodes.Add(address, new BatuhanWalletNode(address));
        _adjacencyList.Add(address, new List<BatuhanTransactionEdge>());
        _addresses.Add(address);
    }

    // 1. Geriye Dönük Akış İçin Gelen Kenarları Bulma Metodu
    public IReadOnlyList<BatuhanTransactionEdge> BatuhanGetIncomingEdges(string address)
    {
        var incomingEdges = new List<BatuhanTransactionEdge>();
        foreach (var walletAddress in _addresses)
        {
            foreach (var edge in BatuhanGetOutgoingEdges(walletAddress))
            {
                if (edge.ToAddress == address)
                {
                    incomingEdges.Add(edge);
                }
            }
        }
        return incomingEdges;
    }

    // 2. İleriye Dönük Fon Akışı (İşlem Döndüren ve Döngü Korumalı BFS)
    public List<BatuhanTransactionEdge> BatuhanGetForwardFundFlow(string startAddress)
    {
        var flowEdges = new List<BatuhanTransactionEdge>();
        // Blokzincirdeki döngüleri (A -> B -> A) kırmak için ID bazlı takip
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new CustomQueue<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return flowEdges;
        }

        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var currentAddress = queue.Dequeue();

            foreach (var edge in BatuhanGetOutgoingEdges(currentAddress))
            {
                if (!visitedEdges.ContainsKey(edge.TransactionId))
                {
                    visitedEdges.Add(edge.TransactionId, true);
                    flowEdges.Add(edge);
                    queue.Enqueue(edge.ToAddress); // Paranın gittiği yeni adresi kuyruğa ekle
                }
            }
        }

        return flowEdges;
    }

    // 3. Geriye Dönük Fon Kaynağı İzleme (İşlem Döndüren BFS)
    public List<BatuhanTransactionEdge> BatuhanGetBackwardFundFlow(string startAddress)
    {
        var flowEdges = new List<BatuhanTransactionEdge>();
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new CustomQueue<string>();

        if (!_nodes.ContainsKey(startAddress))
        {
            return flowEdges;
        }

        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var currentAddress = queue.Dequeue();

            foreach (var edge in BatuhanGetIncomingEdges(currentAddress))
            {
                if (!visitedEdges.ContainsKey(edge.TransactionId))
                {
                    visitedEdges.Add(edge.TransactionId, true);
                    flowEdges.Add(edge);
                    queue.Enqueue(edge.FromAddress); // Paranın geldiği kaynak adresi kuyruğa ekle
                }
            }
        }

        return flowEdges;
    }
}
