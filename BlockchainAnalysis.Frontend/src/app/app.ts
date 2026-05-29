import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterLink } from '@angular/router';
import { GraphEngineComponent, HighlightedPath } from './graph-engine/graph-engine';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, GraphEngineComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
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

  // Cüzdan ID Sorgulama ve Vurgulama Fonksiyonu
  MehmetFindPath() {
    if (!this.startWallet || !this.targetWallet) {
      alert("Lütfen kaynak ve hedef cüzdan adreslerini giriniz!");
      return;
    }

    this.MehmetHighlight = {
      nodes: new Set([this.startWallet, this.targetWallet]),
      edges: new Set([]) // Backend API uçları bağlandığında işlem ID'leri buraya aktarılacaktır
    };

    console.log(`[Mehmet Rota Vurgu]: ${this.startWallet} ve ${this.targetWallet} grafik üzerinde parlatıldı.`);
  }

  // Arayüzü ve grafik görünümünü sıfırlama fonksiyonu
  MehmetClear() {
    this.startWallet = '';
    this.targetWallet = '';
    this.MehmetHighlight = { nodes: new Set(), edges: new Set() };
  }
}
