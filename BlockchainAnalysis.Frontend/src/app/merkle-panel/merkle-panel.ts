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

  @ViewChild('merkleContainer') merkleContainer!: ElementRef;

  public zoomScale: number = 1.0;
  public isFullscreen: boolean = false;
  public translateX: number = 0;
  public translateY: number = 0;
  public isDragging: boolean = false;

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

  @HostListener('document:fullscreenchange', [])
  public onFullscreenChange(): void {
    this.isFullscreen = document.fullscreenElement === this.merkleContainer.nativeElement;
  }

  public BatuhanOnMouseDown(event: MouseEvent): void {
    this.isDragging = true;
    this.startX = event.clientX - this.translateX;
    this.startY = event.clientY - this.translateY;
  }

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
