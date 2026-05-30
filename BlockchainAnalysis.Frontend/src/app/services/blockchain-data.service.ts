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
    // Tüm işlemler tabanda sabit yerlerinde listelenir
    const baseTxs = ['tx001', 'tx002', 'tx003', 'tx004', 'tx005', 'tx006', 'tx007', 'tx008'];

    // Eğer tıklanan tx ilk 8 içinde değilse, görsel düzenin bozulmaması için ilk elemana yerleştirilir
    if (!baseTxs.includes(txId)) {
      baseTxs[0] = txId;
    }

    const getHash = (id: string) => "0x" + id.toUpperCase() + "F";

    // Katman 0: Yapraklar oluşturulur ve hedef seçilir
    const leaves = baseTxs.map(id => ({
      hash: getHash(id),
      label: id,
      state: id === txId ? 'target' : 'default' as any
    }));

    // Katman 1: Alt ikili birleştirmeler ve rota analizi
    const level1 = [];
    for (let i = 0; i < leaves.length; i += 2) {
      const left = leaves[i];
      const right = leaves[i + 1];
      let state = 'default' as any;

      if (left.state === 'target' || right.state === 'target') {
        state = 'computed';
        left.state = left.state === 'target' ? 'target' : 'proof';
        right.state = right.state === 'target' ? 'target' : 'proof';
      }

      level1.push({
        hash: "0xL1_" + (i * 7 + 12).toString(16).toUpperCase(),
        state: state,
        left: left,
        right: right
      });
    }

    // Katman 2: Orta ikili birleştirmeler
    const level2 = [];
    for (let i = 0; i < level1.length; i += 2) {
      const left = level1[i];
      const right = level1[i + 1];
      let state = 'default' as any;

      if (left.state === 'computed' || right.state === 'computed') {
        state = 'computed';
        left.state = left.state === 'computed' ? 'computed' : 'proof';
        right.state = right.state === 'computed' ? 'computed' : 'proof';
      }

      level2.push({
        hash: "0xL2_" + (i * 9 + 55).toString(16).toUpperCase(),
        state: state,
        left: left,
        right: right
      });
    }

    // Katman 3: Kök düğüm birleştirmesi
    const rootNode = {
      hash: "0xROOT_MAIN",
      state: 'root' as any,
      left: level2[0],
      right: level2[1]
    };

    // Kök altındaki dalların ispat durumları eşitlenir
    if (rootNode.left.state === 'computed') rootNode.right.state = 'proof';
    if (rootNode.right.state === 'computed') rootNode.left.state = 'proof';

    return of({
      isValid: true,
      rootHash: "0xROOT_MAIN",
      rootNode: rootNode
    }).pipe(delay(200));
  }
}
