import { Component, Input, ViewEncapsulation, ElementRef, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface BatuhanMerkleNode {
  hash: string;
  label?: string;
  state?: 'default' | 'target' | 'proof' | 'computed' | 'root';
  left?: BatuhanMerkleNode;
  right?: BatuhanMerkleNode;
}

export interface BatuhanMerkleTreeData {
  rootHash: string;
  isValid: boolean;
  rootNode: BatuhanMerkleNode;
}

@Component({
  selector: 'app-merkle-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './merkle-panel.html',
  styleUrls: ['./merkle-panel.css'],
  encapsulation: ViewEncapsulation.None
})
export class BatuhanMerklePanelComponent {
  @Input() public treeData!: BatuhanMerkleTreeData;

  // HTML elemanına erişmek için referans tanımlıyoruz
  @ViewChild('merkleContainer') merkleContainer!: ElementRef;

  public zoomScale: number = 1.0;
  public isFullscreen: boolean = false;

  public BatuhanZoomIn(): void {
    if (this.zoomScale < 2.0) {
      this.zoomScale = Math.round((this.zoomScale + 0.1) * 10) / 10;
    }
  }

  public BatuhanZoomOut(): void {
    if (this.zoomScale > 0.4) {
      this.zoomScale = Math.round((this.zoomScale - 0.1) * 10) / 10;
    }
  }

  public BatuhanResetZoom(): void {
    this.zoomScale = 1.0;
  }

  // Tarayıcının yerleşik tam ekran mekanizmasını tetikleyen fonksiyon
  public BatuhanToggleFullscreen(): void {
    const element = this.merkleContainer.nativeElement;

    if (!document.fullscreenElement) {
      // err parametresine (err: Error) tipi atandı
      element.requestFullscreen().catch((err: Error) => {
        console.error(`Tam ekran hatası: ${err.message}`);
      });
    } else {
      document.exitFullscreen();
    }
  }

  // Kullanıcı ESC tuşuna bastığında veya tam ekrandan çıktığında durumu senkronize eder
  @HostListener('document:fullscreenchange', [])
  public onFullscreenChange(): void {
    this.isFullscreen = document.fullscreenElement === this.merkleContainer.nativeElement;
  }
}
