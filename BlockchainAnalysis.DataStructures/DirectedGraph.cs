using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures;

public class DirectedGraph : IGraph
{
    private readonly HashTable<string, BatuhanWalletNode> _wallets = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly HashTable<string, List<BatuhanTransactionEdge>> _adjacency = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly List<string> _addresses = new();

    public void BatuhanAddVertex(BatuhanWalletNode wallet)
    {
        if (!_wallets.ContainsKey(wallet.Address))
        {
            _wallets.Add(wallet.Address, wallet);
            _adjacency.Add(wallet.Address, new List<BatuhanTransactionEdge>());
            _addresses.Add(wallet.Address);
        }
    }

    public void BatuhanAddEdge(BatuhanTransactionEdge transaction)
    {
        BatuhanAddVertexIfMissing(transaction.FromAddress);
        BatuhanAddVertexIfMissing(transaction.ToAddress);

        _adjacency[transaction.FromAddress].Add(transaction);

        // Yeni modelde ApproximateBalance yerine doğrudan Balance kullanıyoruz
        lock (_wallets[transaction.FromAddress].BalanceLock)
        {
            _wallets[transaction.FromAddress].Balance -= transaction.Amount;
        }

        lock (_wallets[transaction.ToAddress].BalanceLock)
        {
            _wallets[transaction.ToAddress].Balance += transaction.Amount;
        }
    }

    public IReadOnlyList<string> BatuhanGetAddresses() => _addresses;

    public IReadOnlyList<BatuhanTransactionEdge> BatuhanGetOutgoingTransactions(string address)
    {
        if (!_adjacency.TryGetValue(address, out var transactions))
        {
            return Array.Empty<BatuhanTransactionEdge>();
        }

        return transactions;
    }

    public decimal BatuhanGetApproximateBalance(string address)
    {
        return _wallets.TryGetValue(address, out var wallet) ? wallet.Balance : 0m;
    }

    public decimal BatuhanGetIncomingTotal(string address)
    {
        decimal total = 0m;

        foreach (var walletAddress in _addresses)
        {
            foreach (var transaction in BatuhanGetOutgoingTransactions(walletAddress))
            {
                if (transaction.ToAddress == address)
                {
                    total += transaction.Amount;
                }
            }
        }

        return total;
    }

    public decimal BatuhanGetOutgoingTotal(string address)
    {
        decimal total = 0m;

        foreach (var transaction in BatuhanGetOutgoingTransactions(address))
        {
            total += transaction.Amount;
        }

        return total;
    }

    public List<string> BatuhanBreadthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_wallets.ContainsKey(startAddress))
        {
            return order;
        }

        visited.Add(startAddress, true);
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var current = queue.Dequeue();
            order.Add(current);

            foreach (var transaction in BatuhanGetOutgoingTransactions(current))
            {
                if (!visited.ContainsKey(transaction.ToAddress))
                {
                    visited.Add(transaction.ToAddress, true);
                    queue.Enqueue(transaction.ToAddress);
                }
            }
        }

        return order;
    }

    public List<string> BatuhanDepthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var stack = new UmmetStack<string>();

        if (!_wallets.ContainsKey(startAddress))
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

            var outgoing = BatuhanGetOutgoingTransactions(current);
            for (int i = outgoing.Count - 1; i >= 0; i--)
            {
                var nextAddress = outgoing[i].ToAddress;
                if (!visited.ContainsKey(nextAddress))
                {
                    stack.Push(nextAddress);
                }
            }
        }

        return order;
    }

    private void BatuhanAddVertexIfMissing(string address)
    {
        if (!_wallets.ContainsKey(address))
        {
            BatuhanAddVertex(new BatuhanWalletNode(address));
        }
    }

    // 1. Geriye Dönük Akış İçin Gelen Kenarları Bulma Metodu
    public IReadOnlyList<BatuhanTransactionEdge> BatuhanGetIncomingEdges(string address)
    {
        var incomingEdges = new List<BatuhanTransactionEdge>();
        foreach (var walletAddress in _addresses)
        {
            foreach (var edge in BatuhanGetOutgoingTransactions(walletAddress))
            {
                if (edge.ToAddress == address)
                {
                    incomingEdges.Add(edge);
                }
            }
        }
        return incomingEdges;
    }

    public List<BatuhanTransactionEdge> BatuhanGetForwardFundFlow(string startAddress)
    {
        var flowEdges = new List<BatuhanTransactionEdge>();
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_wallets.ContainsKey(startAddress)) return flowEdges;
        queue.Enqueue(startAddress);

        while (!queue.IsEmpty)
        {
            var currentAddress = queue.Dequeue();
            foreach (var transaction in BatuhanGetOutgoingTransactions(currentAddress))
            {
                if (!visitedEdges.ContainsKey(transaction.TransactionId))
                {
                    visitedEdges.Add(transaction.TransactionId, true);
                    flowEdges.Add(transaction);
                    queue.Enqueue(transaction.ToAddress);
                }
            }
        }
        return flowEdges;
    }

    public List<BatuhanTransactionEdge> BatuhanGetBackwardFundFlow(string startAddress)
    {
        var flowEdges = new List<BatuhanTransactionEdge>();
        var visitedEdges = new HashTable<string, bool>(16, WalletHashFunctions.HashFNV1a);
        var queue = new UmmetQueue<string>();

        if (!_wallets.ContainsKey(startAddress)) return flowEdges;
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
                    queue.Enqueue(edge.FromAddress);
                }
            }
        }
        return flowEdges;
    }
    
    public IReadOnlyList<BatuhanTransactionEdge> BatuhanGetOutgoingEdges(string address)
    {
        return BatuhanGetOutgoingTransactions(address);
    }
}