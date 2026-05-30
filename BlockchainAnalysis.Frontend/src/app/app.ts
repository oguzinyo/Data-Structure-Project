import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { GraphEngineComponent, HighlightedPath } from './graph-engine/graph-engine';
import { BlockchainDataService } from './services/blockchain-data.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, GraphEngineComponent],
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

  constructor(private dataService: BlockchainDataService) { }

  ngOnInit() {
    this.loadActivities();
  }

  loadActivities() {
    this.dataService.BatuhanGetRecentActivities().subscribe(data => {
      this.recentActivities = data;
    });
  }

  // Grafikten bir kenara (işleme) tıklandığında çalışır
  BatuhanOnEdgeClicked(edgeData: any) {
    console.log("Seçilen işlem ID:", edgeData.id);

    // Yükleniyor durumu
    this.transactionDetails = { address: "Yükleniyor...", status: "pending" };

    // Backend simülasyonundan veriyi çek
    this.dataService.BatuhanGetTransactionDetails(edgeData.id).subscribe(data => {
      this.transactionDetails = data;
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
  }
}
