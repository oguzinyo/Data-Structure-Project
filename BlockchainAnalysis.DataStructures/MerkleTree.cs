using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAnalysis.DataStructures;

public class MerkleNode
{
    public string Hash { get; set; }
    public MerkleNode? Left { get; set; }
    public MerkleNode? Right { get; set; }

    public MerkleNode(string hash, MerkleNode? left = null, MerkleNode? right = null)
    {
        Hash = hash;
        Left = left;
        Right = right;
    }
}

public class MerkleTree
{
    public MerkleNode? Root { get; private set; }

    public string Build(IReadOnlyList<string> transactionPayloads)
    {
        if (transactionPayloads.Count == 0)
        {
            Root = null;
            return string.Empty;
        }

        var currentLevel = new List<MerkleNode>();
        foreach (var payload in transactionPayloads)
        {
            currentLevel.Add(new MerkleNode(AliComputeHash(payload)));
        }

        while (currentLevel.Count > 1)
        {
            var nextLevel = new List<MerkleNode>();

            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                var left = currentLevel[i];
                var right = i + 1 < currentLevel.Count ? currentLevel[i + 1] : currentLevel[i];
                var combinedHash = AliComputeHash(left.Hash + right.Hash);
                nextLevel.Add(new MerkleNode(combinedHash, left, right));
            }

            currentLevel = nextLevel;
        }

        Root = currentLevel[0];
        return Root.Hash;
    }

    public bool AliVerify(IReadOnlyList<string> transactionPayloads, string expectedRoot)
    {
        return Build(transactionPayloads) == expectedRoot;
    }

    public static string AliComputeHash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var builder = new StringBuilder(bytes.Length * 2);

        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}