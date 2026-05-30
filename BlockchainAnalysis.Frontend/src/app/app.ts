import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { GraphEngineComponent, HighlightedPath } from './graph-engine/graph-engine';
import { BlockchainDataService } from './services/blockchain-data.service';
import { MerklePanelComponent } from './merkle-panel/merkle-panel';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [GraphEngineComponent, MerklePanelComponent],
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

  constructor(private dataService: BlockchainDataService) { }

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
    });

    // Merkle Tree detaylarını çek
    this.dataService.BatuhanGetMerkleTreeData(edgeData.id).subscribe((data: any) => {
      this.currentMerkleData = data;
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
    if (!this.startWallet || !this.targetWallet) return;
    this.MehmetHighlight = {
      nodes: new Set([this.startWallet, this.targetWallet]),
      edges: new Set([])
    };
  }

  MehmetClear() {
    this.startWallet = '';
    this.targetWallet = '';
    this.MehmetHighlight = { nodes: new Set(), edges: new Set() };
    this.transactionDetails = null;
    this.currentMerkleData = null;
  }
}
