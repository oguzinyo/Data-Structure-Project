import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BlockchainDataService {

  constructor() { }

  // İleride HttpClient ile this.http.get('/api/activities') şekline dönüşecek
  getRecentActivities(): Observable<any[]> {
    const mockActivities = [
      { type: 'btc', symbol: '₿', amount: '1,620,828.38', currency: 'USD', asset: 'BTC', time: '15:33 ago' },
      { type: 'eth', symbol: 'Ξ', amount: '4,90,685.33', currency: 'USD', asset: 'ETH', time: '15:32 ago' }
    ];
    return of(mockActivities).pipe(delay(500)); // 500ms ağ gecikmesi simülasyonu
  }

  // İleride HttpClient ile this.http.get('/api/transactions/' + txId) şekline dönüşecek
  getTransactionDetails(txId: string): Observable<any> {
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
}
