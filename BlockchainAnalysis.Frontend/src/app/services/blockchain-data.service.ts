import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BlockchainDataService {

  constructor() { }

  BatuhanGetRecentActivities(): Observable<any[]> {
    const mockActivities = [
      { type: 'btc', symbol: '₿', amount: '1,620,828.38', currency: 'USD', asset: 'BTC', time: '15:33 ago' },
      { type: 'eth', symbol: 'Ξ', amount: '4,90,685.33', currency: 'USD', asset: 'ETH', time: '15:32 ago' }
    ];
    return of(mockActivities).pipe(delay(500));
  }

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

  BatuhanGetMerkleTreeData(txId: string): Observable<any> {
    const allTxs = [
      'tx001', 'tx002', 'tx003', 'tx004', 'tx005', 'tx006', 'tx007',
      'tx008', 'tx009', 'tx010', 'tx011', 'tx012', 'tx013'
    ];

    const paddedLength = 16;
    const baseTxs = [...allTxs];
    while (baseTxs.length < paddedLength) {
      baseTxs.push(baseTxs[baseTxs.length - 1]);
    }

    const getHash = (id: string) => "0x" + id.toUpperCase() + "F";

    // TİP HATASI BURADA ÇÖZÜLDÜ: Değişkene açıkça ': any[]' tipi atandı
    let currentLevel: any[] = baseTxs.map((id, index) => {
      const isTarget = id === txId && allTxs.indexOf(id) === index;
      return {
        hash: getHash(id),
        label: id,
        state: isTarget ? 'target' : 'default'
      };
    });

    let levelCount = 1;
    while (currentLevel.length > 1) {
      // TİP HATASI BURADA ÇÖZÜLDÜ: Ara katman dizisine de ': any[]' atandı
      const nextLevel: any[] = [];

      for (let i = 0; i < currentLevel.length; i += 2) {
        const left = currentLevel[i];
        const right = currentLevel[i + 1];

        let state = 'default';

        if (left.state === 'target' || right.state === 'target' || left.state === 'computed' || right.state === 'computed') {
          state = 'computed';
          if (left.state === 'default') left.state = 'proof';
          if (right.state === 'default') right.state = 'proof';
        }

        nextLevel.push({
          hash: "0xL" + levelCount + "_" + (i * 7 + 12).toString(16).toUpperCase(),
          state: state,
          left: left,
          right: right
        });
      }

      currentLevel = nextLevel;
      levelCount++;
    }

    const rootNode = currentLevel[0];
    rootNode.state = 'root';
    rootNode.hash = "0xROOT_MAIN";

    return of({
      isValid: true,
      rootHash: rootNode.hash,
      rootNode: rootNode
    }).pipe(delay(200));
  }
}
