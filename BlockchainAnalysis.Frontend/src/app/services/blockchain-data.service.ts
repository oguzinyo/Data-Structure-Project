import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BlockchainDataService {

  constructor() { }

  // İleride HttpClient ile this.http.get('/api/activities') şekline dönüşecek
  BatuhanGetRecentActivities(): Observable<any[]> {
    const mockActivities = [
      { type: 'btc', symbol: '₿', amount: '1,620,828.38', currency: 'USD', asset: 'BTC', time: '15:33 ago' },
      { type: 'eth', symbol: 'Ξ', amount: '4,90,685.33', currency: 'USD', asset: 'ETH', time: '15:32 ago' }
    ];
    return of(mockActivities).pipe(delay(500)); // 500ms ağ gecikmesi simülasyonu
  }

  // İleride HttpClient ile this.http.get('/api/transactions/' + txId) şekline dönüşecek
  BatuhanGetTransactionDetails(txId: string): Observable<any> {
    const mockTxDetail = {
      name: "transaction",
      address: txId,
      status: "valid",
      data: {
        amount: Math.floor(Math.random() * 5000) + " BTC",
        volume: Math.floor(Math.random() * 1000) + " GC",
        validator: "Node-" + Math.floor(Math.random() * 9999999)
      }
    };
    return of(mockTxDetail).pipe(delay(300));
  }

  // İleride HttpClient ile this.http.get('/api/merkle/' + txId) şekline dönüşecek
  BatuhanGetMerkleTreeData(txId: string): Observable<any> {
    const mockTree = {
      isValid: true,
      rootNode: {
        hash: "8a7b6c5d4e...",
        left: {
          hash: "1f2e3d4c...",
          left: { hash: txId, data: "Hedef Tx" },
          right: { hash: "9a8b7c6d...", data: "Komşu Tx" }
        },
        right: {
          hash: "5b6a7988...",
          left: { hash: "11223344...", data: "Diğer Tx" },
          right: { hash: "55667788...", data: "Son Tx" }
        }
      }
    };
    return of(mockTree).pipe(delay(400));
  }
}
