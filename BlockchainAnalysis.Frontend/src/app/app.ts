import { Component, OnInit, ViewEncapsulation,ChangeDetectorRef } from '@angular/core';
import { GraphEngineComponent, HighlightedPath } from './graph-engine/graph-engine';
import { BlockchainDataService } from './services/blockchain-data.service';
import { BatuhanMerklePanelComponent } from './merkle-panel/merkle-panel';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [GraphEngineComponent, BatuhanMerklePanelComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
  encapsulation: ViewEncapsulation.None
})
export class App implements OnInit {
  title = 'BlockchainAnalysis.Frontend';

  startWallet: string = '';
  targetWallet: string = '';
  MehmetMinAmount: number = 0;
  MehmetHighlight: HighlightedPath = { nodes: new Set(), edges: new Set() };

  recentActivities: any[] = [];
  transactionDetails: any = null;
  currentMerkleData: any = null; // Mükerrer tanım silindi, sadece burada bırakıldı
  routeMessage: string = '';

  constructor(private dataService: BlockchainDataService, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadActivities();
  }

  loadActivities() {
    this.dataService.BatuhanGetRecentActivities().subscribe((data: any) => {
      this.recentActivities = data;
    });
  }

  // Grafikten bir kenara (işleme) tıklandığında çalışır
  BatuhanOnEdgeClicked(edgeData: any) {
    console.log("Seçilen işlem ID:", edgeData.id);

    this.transactionDetails = { address: "Yükleniyor...", status: "pending" };
    this.currentMerkleData = null; // Eski ağacı temizle

    // İşlem detaylarını çek (Servis isimleri ve any tipleri düzeltildi)
    this.dataService.BatuhanGetTransactionDetails(edgeData.id).subscribe((data: any) => {
      this.transactionDetails = data;
      this.cdr.detectChanges();
    });

    // Merkle Tree detaylarını çek
    this.dataService.BatuhanGetMerkleTreeData(edgeData.id).subscribe((data: any) => {
      this.currentMerkleData = data;
      this.cdr.detectChanges();
    });
  }

  // Grafikten bir düğüme (cüzdana) tıklandığında çalışır
  BatuhanOnNodeClicked(nodeData: any) {
    console.log("Seçilen cüzdan ID:", nodeData.id);
    this.startWallet = nodeData.id; // Sol paneldeki inputu otomatik doldur
  }

  MehmetUpdateFilter(value: string) {
    this.MehmetMinAmount = Number(value);
  }

  MehmetFindPath() {
    /*
    if (!this.startWallet || !this.targetWallet) return;
    this.MehmetHighlight = {
      nodes: new Set([this.startWallet, this.targetWallet]),
      edges: new Set([])
    };*/
    if (!this.startWallet || !this.targetWallet) return;

  const result = this.MehmetBFS(this.startWallet, this.targetWallet);

  if (result) {
    this.MehmetHighlight = result;
    this.routeMessage = '';
  } else {
    this.MehmetHighlight = {
      nodes: new Set([this.startWallet, this.targetWallet]),
      edges: new Set([])
    };
    this.routeMessage = 'Rota bulunamadı';
    console.log('Highlight nodes:', this.startWallet, this.targetWallet);
    console.log('Highlight set:', this.MehmetHighlight);
  }
  this.cdr.detectChanges();

 }
  private MehmetBFS(start: string, target: string): HighlightedPath | null {
    const SYNTHETIC_EDGES = [
      { id: 'tx001', source: '0xA1B2', target: '0xC3D4' },
      { id: 'tx002', source: '0xA1B2', target: '0xE5F6' },
      { id: 'tx003', source: '0xC3D4', target: '0xA1B2' },
      { id: 'tx004', source: '0xE5F6', target: '0xG7H8' },
      { id: 'tx005', source: '0xG7H8', target: '0xO5P6' },
      { id: 'tx006', source: '0xI9J0', target: '0xA1B2' },
      { id: 'tx007', source: '0xI9J0', target: '0xM3N4' },
      { id: 'tx008', source: '0xK1L2', target: '0xE5F6' },
      { id: 'tx009', source: '0xM3N4', target: '0xO5P6' },
      { id: 'tx010', source: '0xQ7R8', target: '0xS9T0' },
      { id: 'tx011', source: '0xS9T0', target: '0xO5P6' },
      { id: 'tx012', source: '0xE5F6', target: '0xI9J0' },
      { id: 'tx013', source: '0xA1B2', target: '0xO5P6' },
    ];
  
    // Klasik BFS - sadece en kısa yol
    const queue: string[] = [start];
    const visited = new Set<string>([start]);
    const parentNode = new Map<string, string>();
    const parentEdge = new Map<string, string>();
  
    while (queue.length > 0) {
      const current = queue.shift()!;
      if (current === target) break;
  
      SYNTHETIC_EDGES
        .filter(e => e.source === current && !visited.has(e.target))
        .forEach(e => {
          visited.add(e.target);
          parentNode.set(e.target, current);
          parentEdge.set(e.target, e.id);
          queue.push(e.target);
        });
    }
  
    if (!parentNode.has(target)) return null;
  
    // Sadece hedef düğüme giden yolu geri izle
    const pathNodes = new Set<string>();
    const pathEdges = new Set<string>();
    let current = target;
  
    while (current !== start) {
      pathNodes.add(current);
      pathEdges.add(parentEdge.get(current)!);
      current = parentNode.get(current)!;
    }
    pathNodes.add(start);
  
    return { nodes: pathNodes, edges: pathEdges };
  }

  MehmetClear() {
    this.startWallet = '';
    this.targetWallet = '';
    this.MehmetHighlight = { nodes: new Set(), edges: new Set() };
    this.transactionDetails = null;
    this.currentMerkleData = null;
    this.routeMessage = '';
  }

}
