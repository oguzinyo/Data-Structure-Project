using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAnalysis.DataStructures;

public class AliMerkleNode
{
    public string Hash { get; set; }
    public AliMerkleNode? Left { get; set; }
    public AliMerkleNode? Right { get; set; }

    public AliMerkleNode(string hash, AliMerkleNode? left = null, AliMerkleNode? right = null)
    {
        Hash = hash;
        Left = left;
        Right = right;
    }
}

public class AliMerkleTree
{
    public AliMerkleNode? Root { get; private set; }

    public string Build(IReadOnlyList<string> transactionPayloads)
    {
        if (transactionPayloads.Count == 0)
        {
            Root = null;
            return string.Empty;
        }

        var currentLevel = new List<AliMerkleNode>();
        foreach (var payload in transactionPayloads)
        {
            currentLevel.Add(new AliMerkleNode(ComputeHash(payload)));
        }

        while (currentLevel.Count > 1)
        {
            var nextLevel = new List<AliMerkleNode>();

            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                var left = currentLevel[i];
                var right = i + 1 < currentLevel.Count ? currentLevel[i + 1] : currentLevel[i];
                var combinedHash = ComputeHash(left.Hash + right.Hash);
                nextLevel.Add(new AliMerkleNode(combinedHash, left, right));
            }

            currentLevel = nextLevel;
        }

        Root = currentLevel[0];
        return Root.Hash;
    }

    public bool Verify(IReadOnlyList<string> transactionPayloads, string expectedRoot)
    {
        return Build(transactionPayloads) == expectedRoot;
    }

    public static string ComputeHash(string value)
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