using BlockchainAnalysis.Core;
using BlockchainAnalysis.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAnalysis.DataStructures;

public class DirectedGraph : IGraph
{
    private readonly HashTable<string, WalletNode> _wallets = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly HashTable<string, List<TransactionEdge>> _adjacency = new(hashFunc: WalletHashFunctions.HashFNV1a);
    private readonly List<string> _addresses = new();

    public void AddVertex(WalletNode wallet)
    {
        if (!_wallets.ContainsKey(wallet.Address))
        {
            _wallets.Add(wallet.Address, wallet);
            _adjacency.Add(wallet.Address, new List<TransactionEdge>());
            _addresses.Add(wallet.Address);
        }
    }

    public void AddEdge(TransactionEdge transaction)
    {
        AddVertexIfMissing(transaction.FromAddress);
        AddVertexIfMissing(transaction.ToAddress);

        _adjacency[transaction.FromAddress].Add(transaction);

        // Yeni modelde ApproximateBalance yerine doðrudan Balance kullanýyoruz
        lock (_wallets[transaction.FromAddress].BalanceLock)
        {
            _wallets[transaction.FromAddress].Balance -= transaction.Amount;
        }

        lock (_wallets[transaction.ToAddress].BalanceLock)
        {
            _wallets[transaction.ToAddress].Balance += transaction.Amount;
        }
    }

    public IReadOnlyList<string> GetAddresses() => _addresses;

    public IReadOnlyList<TransactionEdge> GetOutgoingTransactions(string address)
    {
        if (!_adjacency.TryGetValue(address, out var transactions))
        {
            return Array.Empty<TransactionEdge>();
        }

        return transactions;
    }

    public decimal GetApproximateBalance(string address)
    {
        return _wallets.TryGetValue(address, out var wallet) ? wallet.Balance : 0m;
    }

    public decimal GetIncomingTotal(string address)
    {
        decimal total = 0m;

        foreach (var walletAddress in _addresses)
        {
            foreach (var transaction in GetOutgoingTransactions(walletAddress))
            {
                if (transaction.ToAddress == address)
                {
                    total += transaction.Amount;
                }
            }
        }

        return total;
    }

    public decimal GetOutgoingTotal(string address)
    {
        decimal total = 0m;

        foreach (var transaction in GetOutgoingTransactions(address))
        {
            total += transaction.Amount;
        }

        return total;
    }

    public List<string> BreadthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var queue = new CustomQueue<string>();

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

            foreach (var transaction in GetOutgoingTransactions(current))
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

    public List<string> DepthFirstTraversal(string startAddress)
    {
        var order = new List<string>();
        var visited = new HashTable<string, bool>(hashFunc: WalletHashFunctions.HashFNV1a);
        var stack = new CustomStack<string>();

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

            var outgoing = GetOutgoingTransactions(current);
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

    private void AddVertexIfMissing(string address)
    {
        if (!_wallets.ContainsKey(address))
        {
            AddVertex(new WalletNode(address));
        }
    }
}