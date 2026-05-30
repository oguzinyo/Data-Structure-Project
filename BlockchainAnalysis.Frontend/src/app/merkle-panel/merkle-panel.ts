import { Component, OnInit, Input } from '@angular/core';
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

  @Input() public treeData!: MerkleTreeData;

  ngOnInit(): void {
    // Veri artık app.ts üzerinden geleceği için mock veri yüklemesini kaldırdık.
  }
}
