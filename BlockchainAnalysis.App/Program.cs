using BlockchainAnalysis.DataStructures;
using BlockchainAnalysis.Models;
using System.Globalization;

Console.WriteLine("============================================================");
Console.WriteLine(" BLOKZINCIR ISLEM AGLARI ANALIZI - FAZ 1 DEMO");
Console.WriteLine("============================================================");

var transactions = new[]
{
    new Transaction("0xA1B2", "0xC3D4", 12.50m, 0.10m),
    new Transaction("0xA1B2", "0xE5F6", 4.25m, 0.03m),
    new Transaction("0xC3D4", "0x7788", 2.00m, 0.02m),
    new Transaction("0xE5F6", "0x7788", 1.75m, 0.01m),
    new Transaction("0x7788", "0xA1B2", 0.50m, 0.01m),
};

PrintSection("1) Sentetik Islem Verileri");
foreach (var tx in transactions)
{
    Console.WriteLine($"{ShortId(tx.TransactionId)} | {tx.FromAddress} -> {tx.ToAddress} | Amount: {tx.Amount} | Fee: {tx.Fee} | Time: {tx.Timestamp:HH:mm:ss}");
}

PrintSection("2) Hash Table - Cuzdan ve Islem ID Erisimi");
var walletTable = new HashTable<string, Wallet>(hashFunc: WalletHashFunctions.HashFNV1a);
var transactionTable = new HashTable<string, Transaction>(hashFunc: WalletHashFunctions.HashFNV1a);

foreach (var tx in transactions)
{
    AddWalletIfMissing(walletTable, tx.FromAddress);
    AddWalletIfMissing(walletTable, tx.ToAddress);
    transactionTable.Add(tx.TransactionId, tx);
}

var walletStats = walletTable.GetStats();
var transactionStats = transactionTable.GetStats();
Console.WriteLine($"Cuzdan sayisi: {walletStats.Count}, Bucket: {walletStats.BucketCount}, Load: {walletStats.LoadFactor:F2}, Collision: {walletStats.TotalCollisions}");
Console.WriteLine($"Islem sayisi: {transactionStats.Count}, Bucket: {transactionStats.BucketCount}, Load: {transactionStats.LoadFactor:F2}, Collision: {transactionStats.TotalCollisions}");

var lookupAddress = "0xA1B2";
if (walletTable.TryGetValue(lookupAddress, out var foundWallet))
{
    Console.WriteLine($"O(1) cuzdan erisimi: {foundWallet.Address}");
}

var lookupTransactionId = transactions[2].TransactionId;
if (transactionTable.TryGetValue(lookupTransactionId, out var foundTransaction))
{
    Console.WriteLine($"O(1) islem erisimi: {ShortId(foundTransaction.TransactionId)} -> {foundTransaction.FromAddress} -> {foundTransaction.ToAddress}");
}

PrintSection("3) Yonlu Graf - Transfer Agi");
var graph = new DirectedGraph();
foreach (var tx in transactions)
{
    graph.AddEdge(tx);
}

foreach (var address in graph.GetAddresses())
{
    var incoming = graph.GetIncomingTotal(address);
    var outgoing = graph.GetOutgoingTotal(address);
    var netFlow = incoming - outgoing;
    Console.WriteLine($"{address} | Incoming: {incoming} | Outgoing: {outgoing} | Net Flow: {netFlow}");

    foreach (var edge in graph.GetOutgoingTransactions(address))
    {
        Console.WriteLine($"  -> {edge.ToAddress} | Amount: {edge.Amount} | Time: {edge.Timestamp:HH:mm:ss}");
    }
}

PrintSection("4) Queue ve Stack - Graf Dolasimi");
var bfsOrder = graph.BreadthFirstTraversal("0xA1B2");
var dfsOrder = graph.DepthFirstTraversal("0xA1B2");
Console.WriteLine($"BFS (Queue) from 0xA1B2: {string.Join(" -> ", bfsOrder)}");
Console.WriteLine($"DFS (Stack) from 0xA1B2: {string.Join(" -> ", dfsOrder)}");

PrintSection("5) Merkle Tree - Islem Butunlugu");
var payloads = BuildPayloads(transactions);
var merkleTree = new MerkleTree();
var merkleRoot = merkleTree.Build(payloads);

for (int i = 0; i < payloads.Count; i++)
{
    Console.WriteLine($"Tx Hash {i + 1}: {MerkleTree.ComputeHash(payloads[i])[..16]}...");
}

Console.WriteLine($"Merkle Root: {merkleRoot[..24]}...");
Console.WriteLine($"Dogrulama sonucu: {merkleTree.Verify(payloads, merkleRoot)}");

payloads[0] = payloads[0].Replace("12.50", "99.99");
Console.WriteLine($"Degistirilmis veri dogrulama sonucu: {merkleTree.Verify(payloads, merkleRoot)}");

PrintSection("6) Faz 1 Kontrol Ozeti");
Console.WriteLine("[OK] Directed Graph: Cuzdanlar dugum, transferler yonlu kenar olarak modellendi.");
Console.WriteLine("[OK] Merkle Tree: Islem hashleri ile Merkle Root uretildi ve dogrulandi.");
Console.WriteLine("[OK] Hash Table: Cuzdan adresleri ve islem ID'leri O(1) ortalama erisim icin tutuldu.");
Console.WriteLine("[OK] Queue: BFS dolasimi icin kullanildi.");
Console.WriteLine("[OK] Stack: DFS dolasimi icin kullanildi.");

static void PrintSection(string title)
{
    Console.WriteLine();
    Console.WriteLine(title);
    Console.WriteLine(new string('-', title.Length));
}

static void AddWalletIfMissing(HashTable<string, Wallet> walletTable, string address)
{
    if (!walletTable.ContainsKey(address))
    {
        walletTable.Add(address, new Wallet(address));
    }
}

static List<string> BuildPayloads(IReadOnlyList<Transaction> transactions)
{
    var payloads = new List<string>();

    foreach (var tx in transactions)
    {
        payloads.Add($"{tx.TransactionId}|{tx.FromAddress}|{tx.ToAddress}|{tx.Amount.ToString(CultureInfo.InvariantCulture)}|{tx.Timestamp:O}");
    }

    return payloads;
}

static string ShortId(string value)
{
    return value.Length <= 8 ? value : value[..8];
}
