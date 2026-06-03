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

<<<<<<< Updated upstream
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
=======
  // Tüm durum (state) değişkenlerini tek bir alanda topladık
  public zoomScale: number = 1.0;
  public isFullscreen: boolean = false;
  public translateX: number = 0;
  public translateY: number = 0;
  public isDragging: boolean = false;

  // Özel (private) başlangıç koordinatları
  private startX: number = 0;
  private startY: number = 0;

  public BatuhanZoomIn(): void {
    if (this.zoomScale < 2.0) {
      this.zoomScale = Math.round((this.zoomScale + 0.1) * 10) / 10;
    }
  }

  public BatuhanZoomOut(): void {
    if (this.zoomScale > 0.1) {
      this.zoomScale = Math.round((this.zoomScale - 0.1) * 10) / 10;
    }
  }

  // Tarayıcının yerleşik tam ekran mekanizmasını tetikleyen fonksiyon
  public BatuhanToggleFullscreen(): void {
    const element = this.merkleContainer.nativeElement;

    if (!document.fullscreenElement) {
      element.requestFullscreen().catch((err: Error) => {
        console.error(`Tam ekran hatası: ${err.message}`);
      });
    } else {
      document.exitFullscreen();
    }
  }

  // Kullanıcı ESC tuşuna bastığında veya tam ekrandan çıktığında durumu senkronize eder
  @HostListener('document:fullscreenchange', [])
  public BatuhanOnFullscreenChange(): void {
    this.isFullscreen = document.fullscreenElement === this.merkleContainer.nativeElement;
>>>>>>> Stashed changes
  }

  public BatuhanOnMouseDown(event: MouseEvent): void {
    this.isDragging = true;
    this.startX = event.clientX - this.translateX;
    this.startY = event.clientY - this.translateY;
  }

  // Fareyi bırakma olayını tüm ekranda dinliyoruz ki takılma olmasın
  @HostListener('document:mouseup')
  public BatuhanOnMouseUp(): void {
    this.isDragging = false;
  }

  @HostListener('document:mousemove', ['$event'])
  public BatuhanOnMouseMove(event: MouseEvent): void {
    if (!this.isDragging) return;

    event.preventDefault();
    this.translateX = event.clientX - this.startX;
    this.translateY = event.clientY - this.startY;
  }

  public BatuhanResetZoom(): void {
    this.zoomScale = 1.0;
    this.translateX = 0;
    this.translateY = 0;
  }
}
