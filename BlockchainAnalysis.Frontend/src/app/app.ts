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

  // Dinamik Activity Feed Verileri
  recentActivities = [
    { type: 'btc', symbol: '₿', amount: '1,620,828.38', currency: 'USD', asset: 'BTC', time: '15:33 ago' },
    { type: 'eth', symbol: 'Ξ', amount: '4,90,685.33', currency: 'USD', asset: 'ETH', time: '15:32 ago' }
  ];

  // Dinamik İşlem Detayları
  transactionDetails = {
    name: "transaction",
    address: "826875d02676637ad8...",
    status: "valid",
    data: {
      amount: "3500 BTC",
      volume: "500 GC",
      validator: "18229696..."
    }
  };

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

  // Grafikten bir işleme tıklandığında tetiklenecek metod
  BatuhanOnTransactionSelected(txData: any) {
    this.transactionDetails = {
      name: "transaction",
      address: txData.id || "Bilinmiyor",
      status: "valid",
      data: {
        amount: `${txData.amount} BTC`,
        volume: "Hesaplanıyor...",
        validator: "Ağ Düğümü"
      }
    };
  }
}
