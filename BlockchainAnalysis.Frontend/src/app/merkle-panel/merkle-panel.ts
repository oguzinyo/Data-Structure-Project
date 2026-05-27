import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface MerkleNode {
  hash: string;
  data?: string;
  left?: MerkleNode;
  right?: MerkleNode;
}

export interface MerkleTreeData {
  rootHash: string;
  isValid: boolean;
  rootNode: MerkleNode;
}

@Component({
  selector: 'app-merkle-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './merkle-panel.html',
  styleUrls: ['./merkle-panel.css']
})
export class MerklePanelComponent implements OnInit {

  public treeData!: MerkleTreeData;

  ngOnInit(): void {
    this.loadMockData();
  }

  private loadMockData(): void {
    this.treeData = {
      rootHash: '8a5f3b1c9d2e...',
      isValid: true,
      rootNode: {
        hash: '8a5f3b1c9d2e...',
        left: {
          hash: '4c2d1f9a...',
          left: { hash: 'tx1_hash', data: '0xALICE -> 0xBOB (50)' },
          right: { hash: 'tx2_hash', data: '0xBOB -> 0xCHARLIE (150)' }
        },
        right: {
          hash: '9e7b4d2e...',
          left: { hash: 'tx3_hash', data: '0xALICE -> 0xDAVID (20)' },
          right: undefined
        }
      }
    };
  }
}
