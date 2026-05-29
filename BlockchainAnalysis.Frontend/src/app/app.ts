import { Component, ViewEncapsulation } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { GraphEngineComponent, HighlightedPath } from './graph-engine/graph-engine';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, GraphEngineComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
  encapsulation: ViewEncapsulation.None
})
export class App {
  title = 'BlockchainAnalysis.Frontend';

  startWallet: string = '';
  targetWallet: string = '';
  MehmetMinAmount: number = 0;

  MehmetHighlight: HighlightedPath = { nodes: new Set(), edges: new Set() };

  MehmetUpdateFilter(value: string) {
    this.MehmetMinAmount = Number(value);
    console.log(`[Mehmet UI Filtresi]: Hacim limiti ${this.MehmetMinAmount} BTC olarak güncellendi.`);
  }

  MehmetFindPath() {
    if (!this.startWallet || !this.targetWallet) {
      alert("Lütfen kaynak ve hedef cüzdan adreslerini giriniz!");
      return;
    }

    this.MehmetHighlight = {
      nodes: new Set([this.startWallet, this.targetWallet]),
      edges: new Set([])
    };

    console.log(`[Mehmet Rota Vurgu]: ${this.startWallet} ve ${this.targetWallet} grafik üzerinde parlatıldı.`);
  }

  MehmetClear() {
    this.startWallet = '';
    this.targetWallet = '';
    this.MehmetHighlight = { nodes: new Set(), edges: new Set() };
  }
}
