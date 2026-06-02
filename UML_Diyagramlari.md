# UML Diyagramlari - Blokzincir Islem Aglari Analizi

> Bu dosyadaki Mermaid diyagramlarini gorsellestirmek icin:
> - [Mermaid Live Editor](https://mermaid.live) adresine yapistirin
> - Veya GitHub uzerinde direkt goruntulenebilir

---

## 1. Sinif Diyagrami (Class Diagram)

```mermaid
classDiagram
    class IGraph {
        <<interface>>
        +BatuhanAddVertex(wallet: BatuhanWalletNode)
        +BatuhanAddEdge(transaction: BatuhanTransactionEdge)
        +BatuhanGetForwardFundFlow(startAddress: string) List~BatuhanTransactionEdge~
        +BatuhanGetBackwardFundFlow(startAddress: string) List~BatuhanTransactionEdge~
        +BatuhanGetOutgoingEdges(address: string) IReadOnlyList~BatuhanTransactionEdge~
        +BatuhanGetIncomingEdges(address: string) IReadOnlyList~BatuhanTransactionEdge~
    }

    class BatuhanWalletNode {
        +Address: string
        +Balance: decimal
        +BalanceLock: object
        +AddFunds(amount: decimal)
        +DeductFunds(amount: decimal)
        +GetCurrentBalance() decimal
    }

    class BatuhanTransactionEdge {
        +TransactionId: string
        +From: BatuhanWalletNode
        +To: BatuhanWalletNode
        +Amount: decimal
        +Fee: decimal
        +Timestamp: DateTime
        +FromAddress: string
        +ToAddress: string
    }

    class BlockchainGraph {
        -_nodes: HashTable~string, BatuhanWalletNode~
        -_adjacencyList: HashTable~string, List~BatuhanTransactionEdge~~
        -_addresses: List~string~
        -_graphLock: object
        +BatuhanAddVertex(wallet)
        +BatuhanAddEdge(edge)
        +BatuhanBreadthFirstTraversal(startAddress) List~string~
        +BatuhanDepthFirstTraversal(startAddress) List~string~
        +BatuhanGetForwardFundFlow(startAddress) List~BatuhanTransactionEdge~
        +BatuhanGetBackwardFundFlow(startAddress) List~BatuhanTransactionEdge~
        +MehmetFindPath(start, target) List~string~
        +MehmetFindMaxCapacityPath(start, target) List~string~
    }

    class HashTable~TKey, TValue~ {
        -_buckets: List~HashEntry~[]
        -_count: int
        -_totalCollisions: int
        +Add(key, value)
        +TryGetValue(key, out value) bool
        +Remove(key) bool
        +ContainsKey(key) bool
        +GetStats() HashTableStats
    }

    class UmmetQueue~T~ {
        -_items: T[]
        -_head: int
        -_tail: int
        -_count: int
        +Enqueue(item: T)
        +Dequeue() T
        +Count: int
        +IsEmpty: bool
    }

    class UmmetStack~T~ {
        -_items: T[]
        -_count: int
        +Push(item: T)
        +Pop() T
        +Count: int
        +IsEmpty: bool
    }

    class AliMerkleTree {
        +Root: AliMerkleNode
        +Build(payloads) string
        +Verify(payloads, expectedRoot) bool
        +ComputeHash(value) string
    }

    class FundFlowTracker {
        -_graph: IGraph
        +BatuhanTrackForwardFlow(startAddress, filters) List~BatuhanTransactionEdge~
        +BatuhanTrackBackwardFlow(endAddress, filters) List~BatuhanTransactionEdge~
    }

    class UmmetDynamicBalanceEngine {
        -_graph: IGraph
        +UmmetUpdateBalanceSafely(wallet, amount, isIncoming)
        +UmmetCalculateDynamicBalance(walletAddress) decimal
    }

    class AliSyntheticDataGenerator {
        -_random: Random
        -_walletsHashTable: HashTable
        -_transactionsHashTable: HashTable
        +GenerateWallets(count) List
        +GenerateRandomTransactions() List
        +GenerateChainFlow() List
        +GenerateExchangeScenario() List
        +GenerateCycleScenario() List
    }

    IGraph <|.. BlockchainGraph
    BlockchainGraph --> HashTable : uses
    BlockchainGraph --> UmmetQueue : BFS
    BlockchainGraph --> UmmetStack : DFS
    BlockchainGraph --> BatuhanWalletNode : stores
    BlockchainGraph --> BatuhanTransactionEdge : stores
    BatuhanTransactionEdge --> BatuhanWalletNode : From/To
    FundFlowTracker --> IGraph : depends on
    UmmetDynamicBalanceEngine --> IGraph : depends on
    AliMerkleTree --> AliMerkleNode : builds
    AliSyntheticDataGenerator --> HashTable : uses
    AliSyntheticDataGenerator --> BatuhanWalletNode : generates
    AliSyntheticDataGenerator --> BatuhanTransactionEdge : generates
```

---

## 2. BFS Algoritmasi Sekans Diyagrami

```mermaid
sequenceDiagram
    participant Client
    participant Graph as BlockchainGraph
    participant Queue as UmmetQueue
    participant HT as HashTable (visited)

    Client->>Graph: BatuhanBreadthFirstTraversal("0xA1B2")
    Graph->>HT: Add("0xA1B2", true)
    Graph->>Queue: Enqueue("0xA1B2")
    
    loop Queue bos olana kadar
        Graph->>Queue: Dequeue() → current
        Graph->>Graph: order.Add(current)
        Graph->>Graph: BatuhanGetOutgoingEdges(current)
        loop Her komsu kenar icin
            Graph->>HT: ContainsKey(neighbor)?
            alt Ziyaret edilmemis
                Graph->>HT: Add(neighbor, true)
                Graph->>Queue: Enqueue(neighbor)
            end
        end
    end
    
    Graph-->>Client: order listesi doner
```

---

## 3. Ileriye Donuk Fon Akisi Sekans Diyagrami

```mermaid
sequenceDiagram
    participant Client
    participant Tracker as FundFlowTracker
    participant Graph as BlockchainGraph
    participant Queue as UmmetQueue
    participant HT as HashTable (visitedEdges)

    Client->>Tracker: BatuhanTrackForwardFlow("0xALICE", minAmount:100)
    Tracker->>Graph: BatuhanGetForwardFundFlow("0xALICE")
    Graph->>Queue: Enqueue("0xALICE")
    
    loop Queue bos olana kadar
        Graph->>Queue: Dequeue() → currentAddress
        Graph->>Graph: BatuhanGetOutgoingEdges(currentAddress)
        loop Her islem kenari icin
            Graph->>HT: ContainsKey(edge.TransactionId)?
            alt Islem daha once izlenmemis
                Graph->>HT: Add(edge.TransactionId, true)
                Note over Graph: flowEdges.Add(edge)
                Graph->>Queue: Enqueue(edge.ToAddress)
            end
        end
    end
    
    Graph-->>Tracker: flowEdges doner
    Tracker->>Tracker: BatuhanApplyFilters(minAmount>=100)
    Tracker-->>Client: filtrelenmis sonuc
```

---

## 4. Merkle Tree Build Sekans Diyagrami

```mermaid
sequenceDiagram
    participant Client
    participant MT as AliMerkleTree
    participant SHA as SHA-256

    Client->>MT: Build([tx1, tx2, tx3, tx4])
    
    loop Her transaction payload icin
        MT->>SHA: ComputeHash(payload)
        SHA-->>MT: leaf hash
        Note over MT: Yaprak dugum olustur
    end
    
    loop currentLevel.Count > 1
        Note over MT: Seviye seviye yukari cik
        loop i = 0, 2, 4, ...
            MT->>MT: left = currentLevel[i]
            MT->>MT: right = currentLevel[i+1]
            MT->>SHA: ComputeHash(left.Hash + right.Hash)
            SHA-->>MT: parent hash
            Note over MT: Yeni dugum(hash, left, right)
        end
        Note over MT: currentLevel = nextLevel
    end
    
    MT-->>Client: Root.Hash (Merkle Root)
```
