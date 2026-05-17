# Blokzincir Islem Aglari Analizi

Bu proje, blokzincir sistemlerindeki islem verilerini sade bir model uzerinden analiz etmek icin hazirlanmis bir Veri Yapilari projesidir. Cuzdan adresleri graf dugumu, para transferleri ise miktar ve zaman bilgisi tasiyan yonlu kenar olarak modellenir.

## Proje Amaci

Projenin temel amaci, kripto para islem agini temel veri yapilari ile temsil etmek ve terminal uzerinden calisan bir Faz 1 demosu sunmaktir.

Faz 1 kapsaminda su yapilar calisir durumdadir:

- Yonlu graf ile cuzdanlar arasi transfer agi modellenir.
- Hash table ile cuzdan adreslerine ve islem ID'lerine ortalama O(1) erisim saglanir.
- Merkle tree ile islem verilerinden Merkle Root uretilir ve veri butunlugu dogrulanir.
- Queue ile BFS dolasimi yapilir.
- Stack ile DFS dolasimi yapilir.
- Gelen, giden ve net fon akisi hesaplanir.

## Proje Mimarisi

```text
BlockchainAnalysis.App/
  Program.cs

BlockchainAnalysis.Core/
  IGraph.cs
  IHashTable.cs

BlockchainAnalysis.DataStructures/
  BlockchainGraph.cs
  CustomQueue.cs
  CustomStack.cs
  DirectedGraph.cs
  HashTable.cs
  MerkleTree.cs

BlockchainAnalysis.Models/
  GraphNode.cs
  TransactionEdge.cs
  Transaction.cs
  Wallet.cs
  WalletNode.cs

BlockchainAnalysis.slnx
Dockerfile
docker-compose.yml
```

## Katmanlar

- `BlockchainAnalysis.App`: Konsol uygulamasi ve Faz 1 demo akisinin basladigi katman.
- `BlockchainAnalysis.Core`: Veri yapilari icin ortak arayuzleri barindirir.
- `BlockchainAnalysis.DataStructures`: Projede kullanilan temel veri yapilarini icerir.
- `BlockchainAnalysis.Models`: Cuzdan ve islem modellerini icerir.

## Veri Yapilari

### BlockchainGraph

`BlockchainGraph`, kripto islem agini node-edge modeliyle yonlu graf olarak temsil eder.

- Dugumler `GraphNode` sinifi ile temsil edilir.
- Kenarlar `TransactionEdge` sinifi ile temsil edilir.
- Kenarlarda transfer miktari ve zaman bilgisi bulunur.
- Graf dongu icerebilir. Ornek: `0x7788 -> 0xA1B2` transferi ile para akisi tekrar baslangic adresine donebilir.

Node ve edge iliskisi adjacency list ile kurulur:

```text
BlockchainGraph
  nodes[address] -> GraphNode
  adjacencyList[fromAddress] -> List<TransactionEdge>
```

Ornek:

```text
GraphNode(0xA1B2)
  Edge: 0xA1B2 -> 0xC3D4 | Amount: 12,50
  Edge: 0xA1B2 -> 0xE5F6 | Amount: 4,25
```

Demo ciktisinda her cuzdan icin su bilgiler gosterilir:

```text
0xA1B2 | Incoming: 0,50 | Outgoing: 16,75 | Net Flow: -16,25
```

Buradaki `Net Flow`, gercek blokzincir bakiyesi degil, basitlestirilmis modeldeki `gelen transferler - giden transferler` sonucudur.

### Hash Table

`HashTable<TKey, TValue>`, separate chaining yaklasimi ile collision yonetimi yapan generic bir hash table implementasyonudur.

Ozellikler:

- Ortalama O(1) ekleme ve arama hedeflenir.
- Cuzdan adresleri icin kullanilir.
- Islem ID'leri icin kullanilir.
- Load factor, collision ve bucket istatistikleri terminalde gosterilir.

### Merkle Tree

`MerkleTree`, islem payload'larini SHA-256 ile hashler ve ikili agac yapisinda birlestirerek Merkle Root uretir.

Demo akisinda:

- Her islem icin hash uretilir.
- Merkle Root hesaplanir.
- Orijinal veri dogrulanir.
- Veri degistirildiginde dogrulamanin basarisiz oldugu gosterilir.

### Queue ve Stack

Projede BFS ve DFS icin custom veri yapilari kullanilir.

- `CustomQueue<T>`: BFS icin FIFO mantigiyla calisir.
- `CustomStack<T>`: DFS icin LIFO mantigiyla calisir.

## Terminal Demo Akisi

Uygulama calistirildiginda su bolumler terminalde gosterilir:

1. Sentetik islem verileri
2. Hash table ile cuzdan ve islem ID erisimi
3. Yonlu graf ile transfer agi
4. Queue ve Stack ile graf dolasimi
5. Merkle tree ile islem butunlugu
6. Faz 1 kontrol ozeti

Ornek graf ciktisi:

```text
0xA1B2 | Incoming: 0,50 | Outgoing: 16,75 | Net Flow: -16,25
  -> 0xC3D4 | Amount: 12,50 | Time: 12:25:11
  -> 0xE5F6 | Amount: 4,25 | Time: 12:25:11
```

Ornek dolasim ciktisi:

```text
BFS (Queue) from 0xA1B2: 0xA1B2 -> 0xC3D4 -> 0xE5F6 -> 0x7788
DFS (Stack) from 0xA1B2: 0xA1B2 -> 0xC3D4 -> 0x7788 -> 0xE5F6
```

## Gereksinimler

- .NET SDK 10.0
- Docker Desktop, Docker ile calistirmak istenirse

Kurulu SDK'lari kontrol etmek icin:

```bash
dotnet --list-sdks
```

## Calistirma

Projeyi derlemek icin:

```bash
dotnet build BlockchainAnalysis.slnx
```

Konsol uygulamasini calistirmak icin:

```bash
dotnet run --project BlockchainAnalysis.App/BlockchainAnalysis.App.csproj
```

Windows PowerShell icin alternatif komut:

```powershell
dotnet run --project "BlockchainAnalysis.App\BlockchainAnalysis.App.csproj"
```

## Docker Ile Calistirma

Docker uzerinden calistirmak icin:

```bash
docker-compose up --build
```

Container'i durdurmak icin:

```bash
docker-compose down
```

## Entegre Edilen Branch Icerikleri

- `setup-architecture`: Solution, proje klasorleri, Docker dosyalari ve temel modeller.
- `feature/hash-table`: Generic hash table implementasyonu ve wallet hash fonksiyonlari.
- `feature-directed-graph`: Graf dugumu olarak kullanilacak wallet node modeli.
- `feature-merkle-tree`: Merkle agaci baslangic dugum modeli.
- `feature-queue-stack`: BFS ve DFS için $O(1)$ performanslı ve thread-safe veri yapıları.

## Faz 1 Durumu

- `[OK]` Directed Graph
- `[OK]` BlockchainGraph node-edge modeli
- `[OK]` Merkle Tree
- `[OK]` Hash Table
- `[OK]` Queue
- `[OK]` Stack

## Takim Uyeleri

- OGUZHAN HEKIMOGLU
- MEHMET OGUZHAN TANRIVERDI
- BATUHAN OZDEMIR
- ALI KADIR OZYASAR
- UMMET ERKAN

## Notlar

- `bin/`, `obj/` ve `.vs/` klasorleri repoya dahil edilmez.
- Terminalde gosterilen bakiye degeri `Net Flow` olarak hesaplanir.
- Bu proje su an konsol uygulamasi olarak calisir; Faz 3 arayuz gereksinimleri henuz uygulanmamistir.
