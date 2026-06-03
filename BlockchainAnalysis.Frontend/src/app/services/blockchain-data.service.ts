import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

const API_BASE = 'http://localhost:5050/api';

@Injectable({
  providedIn: 'root'
})
export class BlockchainDataService {

  constructor(private http: HttpClient) { }

  BatuhanGetGraphData(): Observable<any> {
    return this.http.get<any>(`${API_BASE}/graph`).pipe(
      catchError(err => {
        console.error('Graf verisi alinamadi:', err);
        return of({ nodes: [], edges: [] });
      })
    );
  }

  BatuhanGetRecentActivities(): Observable<any[]> {
    return this.http.get<any[]>(`${API_BASE}/graph/edges`).pipe(
      map(edges => {
        return edges.slice(0, 5).map((e: any) => ({
          type: 'btc',
          symbol: '₿',
          amount: e.amount.toLocaleString('en-US', { minimumFractionDigits: 2 }),
          currency: 'BTC',
          asset: 'BTC',
          time: new Date(e.timestamp).toLocaleTimeString('tr-TR')
        }));
      }),
      catchError(() => of([]))
    );
  }

  BatuhanGetTransactionDetails(txId: string): Observable<any> {
    return this.http.get<any[]>(`${API_BASE}/graph/edges`).pipe(
      map(edges => {
        const tx = edges.find((e: any) => e.id === txId);
        if (!tx) return { name: "transaction", address: txId, status: "not_found", data: {} };
        return {
          name: "transaction",
          address: tx.id,
          status: "valid",
          data: {
            amount: tx.amount + " BTC",
            fee: tx.fee + " BTC",
            source: tx.source,
            target: tx.target,
            timestamp: new Date(tx.timestamp).toLocaleString('tr-TR')
          }
        };
      }),
      catchError(() => of({ name: "transaction", address: txId, status: "error", data: {} }))
    );
  }

  BatuhanGetMerkleTreeData(txId: string): Observable<any> {
    return this.http.get<any>(`${API_BASE}/merkle/tree`, {
      params: { transactionId: txId }
    }).pipe(
      catchError(() => of(null))
    );
  }

  BatuhanGetForwardFlow(startAddress: string, minAmount?: number): Observable<any> {
    const params: any = { startAddress };
    if (minAmount) params.minAmount = minAmount;
    return this.http.get<any>(`${API_BASE}/analysis/flow`, { params }).pipe(
      catchError(() => of({ bfsOrder: [], dfsOrder: [], flowEdges: [] }))
    );
  }

  BatuhanGetBackwardFlow(startAddress: string): Observable<any> {
    return this.http.get<any>(`${API_BASE}/analysis/flow/backward`, {
      params: { startAddress }
    }).pipe(
      catchError(() => of({ flowEdges: [] }))
    );
  }

  MehmetFindPath(startAddress: string, targetAddress: string): Observable<any> {
    return this.http.get<any>(`${API_BASE}/analysis/path`, {
      params: { startAddress, targetAddress }
    }).pipe(
      catchError(() => of({ path: [], found: false }))
    );
  }

  MehmetFindMaxCapacityPath(startAddress: string, targetAddress: string): Observable<any> {
    return this.http.get<any>(`${API_BASE}/analysis/path/max-capacity`, {
      params: { startAddress, targetAddress }
    }).pipe(
      catchError(() => of({ path: [], found: false }))
    );
  }

  BatuhanGetDynamicBalance(walletAddress: string): Observable<any> {
    return this.http.get<any>(`${API_BASE}/analysis/balance`, {
      params: { walletAddress }
    }).pipe(
      catchError(() => of({ dynamicBalance: 0 }))
    );
  }
}
