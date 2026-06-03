import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef, NgZone, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { GraphEngineComponent, GraphData, HighlightedPath } from './graph-engine/graph-engine';
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

  graphData: GraphData = { nodes: [], edges: [] };

  startWallet: string = '';
  targetWallet: string = '';
  MehmetMinAmount: number = 0;
  MehmetHighlight: HighlightedPath = { nodes: new Set(), edges: new Set() };

  recentActivities: any[] = [];
  transactionDetails: any = null;
  currentMerkleData: any = null;

  private isBrowser: boolean;

  constructor(
    private dataService: BlockchainDataService,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit() {
    if (!this.isBrowser) return;
    this.loadGraphData();
    this.loadActivities();
  }

  loadGraphData() {
    this.dataService.BatuhanGetGraphData().subscribe((data: any) => {
      this.graphData = data;
      this.cdr.detectChanges();
    });
  }

  loadActivities() {
    this.dataService.BatuhanGetRecentActivities().subscribe((data: any) => {
      this.recentActivities = data;
    });
  }

  BatuhanOnEdgeClicked(edgeData: any) {
    this.ngZone.run(() => {
      const src = typeof edgeData.source === 'object' ? edgeData.source.id : edgeData.source;
      const tgt = typeof edgeData.target === 'object' ? edgeData.target.id : edgeData.target;

      this.transactionDetails = {
        address: edgeData.id,
        status: "valid",
        data: {
          amount: edgeData.amount + " BTC",
          fee: (edgeData.fee ?? 0) + " BTC",
          source: src,
          target: tgt,
          timestamp: new Date(edgeData.timestamp).toLocaleString('tr-TR')
        }
      };
      this.currentMerkleData = null;
      this.cdr.detectChanges();

      this.dataService.BatuhanGetMerkleTreeData(edgeData.id).subscribe((data: any) => {
        this.ngZone.run(() => {
          this.currentMerkleData = data;
          this.cdr.detectChanges();
        });
      });
    });
  }

  BatuhanOnNodeClicked(nodeData: any) {
    this.ngZone.run(() => {
      this.startWallet = nodeData.id;
      this.transactionDetails = null;
      this.currentMerkleData = null;
      this.cdr.detectChanges();
    });
  }

  onSelectionCleared() {
    this.ngZone.run(() => {
      this.transactionDetails = null;
      this.currentMerkleData = null;
      this.cdr.detectChanges();
    });
  }

  MehmetUpdateFilter(value: string) {
    this.MehmetMinAmount = Number(value);
  }

  MehmetFindPath() {
    if (!this.startWallet || !this.targetWallet) return;

    this.dataService.MehmetFindPath(this.startWallet, this.targetWallet).subscribe((result: any) => {
      if (result.found && result.path.length > 0) {
        const pathNodes = new Set<string>(result.path);
        const pathEdges = new Set<string>();

        for (let i = 0; i < result.path.length - 1; i++) {
          const from = result.path[i];
          const to = result.path[i + 1];
          const edge = this.graphData.edges.find((e: any) => {
            const src = typeof e.source === 'object' ? (e.source as any).id : e.source;
            const tgt = typeof e.target === 'object' ? (e.target as any).id : e.target;
            return src === from && tgt === to;
          });
          if (edge) pathEdges.add(edge.id);
        }

        this.MehmetHighlight = { nodes: pathNodes, edges: pathEdges };
      } else {
        this.MehmetHighlight = {
          nodes: new Set([this.startWallet, this.targetWallet]),
          edges: new Set()
        };
      }
      this.cdr.detectChanges();
    });
  }

  MehmetClear() {
    this.startWallet = '';
    this.targetWallet = '';
    this.MehmetHighlight = { nodes: new Set(), edges: new Set() };
    this.transactionDetails = null;
    this.currentMerkleData = null;
  }
}
