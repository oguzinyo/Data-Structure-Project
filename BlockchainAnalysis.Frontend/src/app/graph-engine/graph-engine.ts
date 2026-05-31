import {
  Component,
  OnInit,
  AfterViewInit,
  OnChanges,
  OnDestroy,
  ElementRef,
  ViewChild,
  Input,
  Output,
  EventEmitter ,
  SimpleChanges,
  ChangeDetectorRef,
} from '@angular/core';
import * as d3 from 'd3';


import { DatePipe } from '@angular/common';

export interface WalletNode {
  id: string;
  label: string;
  balance: number;
  x?: number;
  y?: number;
  fx?: number | null;
  fy?: number | null;
}

export interface TransactionEdge {
  id: string;
  source: string | WalletNode;
  target: string | WalletNode;
  amount: number;
  timestamp: string;
}

export interface GraphData {
  nodes: WalletNode[];
  edges: TransactionEdge[];
}

export interface HighlightedPath {
  nodes: Set<string>;
  edges: Set<string>;
}

const SYNTHETIC_DATA: GraphData = {
  nodes: [
    { id: '0xA1B2', label: '0xA1B2', balance: 4200 },
    { id: '0xC3D4', label: '0xC3D4', balance: 870 },
    { id: '0xE5F6', label: '0xE5F6', balance: 3100 },
    { id: '0xG7H8', label: '0xG7H8', balance: 150 },
    { id: '0xI9J0', label: '0xI9J0', balance: 5600 },
    { id: '0xK1L2', label: '0xK1L2', balance: 720 },
    { id: '0xM3N4', label: '0xM3N4', balance: 2300 },
    { id: '0xO5P6', label: 'BORSA', balance: 9800 },
    { id: '0xQ7R8', label: '0xQ7R8', balance: 430 },
    { id: '0xS9T0', label: '0xS9T0', balance: 1100 },
  ],
  edges: [
    { id: 'tx001', source: '0xA1B2', target: '0xC3D4', amount: 500,  timestamp: '2024-01-15T10:22:00Z' },
    { id: 'tx002', source: '0xA1B2', target: '0xE5F6', amount: 1200, timestamp: '2024-01-15T11:05:00Z' },
    { id: 'tx003', source: '0xC3D4', target: '0xA1B2', amount: 200,  timestamp: '2024-01-16T09:10:00Z' },
    { id: 'tx004', source: '0xE5F6', target: '0xG7H8', amount: 80,   timestamp: '2024-01-16T14:30:00Z' },
    { id: 'tx005', source: '0xG7H8', target: '0xO5P6', amount: 70,   timestamp: '2024-01-17T08:00:00Z' },
    { id: 'tx006', source: '0xI9J0', target: '0xA1B2', amount: 900,  timestamp: '2024-01-17T12:45:00Z' },
    { id: 'tx007', source: '0xI9J0', target: '0xM3N4', amount: 650,  timestamp: '2024-01-18T16:00:00Z' },
    { id: 'tx008', source: '0xK1L2', target: '0xE5F6', amount: 300,  timestamp: '2024-01-18T17:30:00Z' },
    { id: 'tx009', source: '0xM3N4', target: '0xO5P6', amount: 400,  timestamp: '2024-01-19T10:00:00Z' },
    { id: 'tx010', source: '0xQ7R8', target: '0xS9T0', amount: 150,  timestamp: '2024-01-19T11:20:00Z' },
    { id: 'tx011', source: '0xS9T0', target: '0xO5P6', amount: 130,  timestamp: '2024-01-20T09:00:00Z' },
    { id: 'tx012', source: '0xE5F6', target: '0xI9J0', amount: 250,  timestamp: '2024-01-20T13:15:00Z' },
    { id: 'tx013', source: '0xA1B2', target: '0xO5P6', amount: 700,  timestamp: '2024-01-21T08:30:00Z' },
  ],
};

const C = {
  bg: '#070d1a', panel: '#0d1526', border: '#1a2d4a',
  nd: '#1e3a5f', nstr: '#2dd4bf', nhl: '#f59e0b', ndim: '#0a1220',
  ed: '#2563eb', ehl: '#f59e0b', edim: '#0a1220',
  txt: '#94a3b8', txth: '#f1f5f9',
  exc: '#dc2626', acc: '#2dd4bf',
};

const clamp = (v: number, lo: number, hi: number) => Math.min(Math.max(v, lo), hi);



function nRadius(bal: number, all: number[]): number {
  const mn = Math.min(...all), mx = Math.max(...all);
  const t = mx === mn ? 0.5 : (bal - mn) / (mx - mn);
  return clamp(10 + t * 30, 10, 40);
}

function eWidth(amt: number, all: number[]): number {
  const mn = Math.min(...all), mx = Math.max(...all);
  const t = mx === mn ? 0.5 : (amt - mn) / (mx - mn);
  return clamp(1.5 + t * 7, 1.5, 8.5);
}

@Component({
  selector: 'app-graph-engine',
  standalone: true,

  imports: [DatePipe],

  templateUrl: './graph-engine.html',
  host: { style: 'display:block; width:100%; height:100%;' },
  styleUrl: './graph-engine.css',
})
export class GraphEngineComponent implements AfterViewInit, OnChanges, OnDestroy {

  private currentEdges: TransactionEdge[] = [];


  @ViewChild('svgContainer', { static: true }) svgContainer!: ElementRef<SVGSVGElement>;

  @Input() graphData: GraphData = SYNTHETIC_DATA;
  @Input() highlightedPath: HighlightedPath = { nodes: new Set(), edges: new Set() };

  @Output() nodeClicked = new EventEmitter<any>();
  @Output() edgeClicked = new EventEmitter<any>();

  tooltip: { x: number; y: number; lines: string[] } | null = null;
  selectedEdge: TransactionEdge | null = null;
  selectedNode: WalletNode | null = null;

  private simulation: d3.Simulation<WalletNode, TransactionEdge> | null = null;
  private resizeObserver: ResizeObserver | null = null;
  private width = 800;
  private height = 600;



  constructor(private cdr: ChangeDetectorRef) {}




  ngAfterViewInit(): void {
    if (typeof window === 'undefined') return;
    this.observeSize();
    this.renderGraph();
    
  }





  selectedNodeTxs: TransactionEdge[] = [];

  isSender(tx: TransactionEdge): boolean {
    const src = typeof tx.source === 'object' ? (tx.source as WalletNode).id : tx.source;
    return src === this.selectedNode?.id;
  }
  getNodeId(node: string | WalletNode): string {
    return typeof node === 'object' ? node.id : node;
  }






  ngOnChanges(changes: SimpleChanges): void {
    if (changes['graphData'] || changes['highlightedPath']) {
      this.renderGraph();
    }
  }

  ngOnDestroy(): void {
    this.simulation?.stop();
    this.resizeObserver?.disconnect();
  }

  private observeSize(): void {
    if (typeof ResizeObserver === 'undefined') return;
    const el = this.svgContainer.nativeElement.parentElement!;
    this.resizeObserver = new ResizeObserver(([entry]) => {
      this.width = entry.contentRect.width;
      this.height = entry.contentRect.height;
      this.renderGraph();
    });
    this.resizeObserver.observe(el);
  }

  clearSelection(): void {
    this.selectedEdge = null;
    this.selectedNode = null;
    this.cdr.detectChanges();
  }

  private renderGraph(): void {
    const svgEl = this.svgContainer.nativeElement;
    
    const w = this.width || 800;
    const h = this.height || 600;

    

    const svg = d3.select(svgEl);
    svg.selectAll('*').remove();

    const nodes: WalletNode[] = this.graphData.nodes.map(n => ({ ...n }));
    const edges: TransactionEdge[] = this.graphData.edges.map(e => ({ ...e }));
    this.currentEdges = edges;

    const allBal = nodes.map(n => n.balance);
    const allAmt = this.graphData.edges.map(e => e.amount);
    const hlPath = this.highlightedPath;
    const hasHL = hlPath.nodes.size > 0;

    // Çift yönlü kenar tespiti
    const biMap = new Set<string>();
    edges.forEach(e => {
      const src = e.source as string;
      const tgt = e.target as string;
      if (edges.find(x => x.source === tgt && x.target === src)) {
        biMap.add(`${src}->${tgt}`);
      }
    });

    // Defs: arrow markers + glow filtresi
    const defs = svg.append('defs');

    ([['arr-d', C.ed], ['arr-h', C.ehl], ['arr-dim', '#1a2d4a']] as [string, string][])
      .forEach(([id, clr]) => {
        defs.append('marker').attr('id', id)
          .attr('viewBox', '0 -5 10 10').attr('refX', 10).attr('refY', 0)
          .attr('markerWidth', 5).attr('markerHeight', 5).attr('orient', 'auto')
          .append('path').attr('d', 'M0,-5L10,0L0,5').attr('fill', clr);
      });

    const gf = defs.append('filter').attr('id', 'glow')
      .attr('x', '-50%').attr('y', '-50%').attr('width', '200%').attr('height', '200%');
    gf.append('feGaussianBlur').attr('stdDeviation', '3').attr('result', 'blur');
    const fm = gf.append('feMerge');
    fm.append('feMergeNode').attr('in', 'blur');
    fm.append('feMergeNode').attr('in', 'SourceGraphic');

    // Zemin + grid
    svg.append('rect').attr('width', w).attr('height', h).attr('fill', C.bg);
    const grid = svg.append('g');
    for (let x = 0; x < w; x += 50)
      grid.append('line').attr('x1', x).attr('y1', 0).attr('x2', x).attr('y2', h)
        .attr('stroke', '#0b1628').attr('stroke-width', 1);
    for (let y = 0; y < h; y += 50)
      grid.append('line').attr('x1', 0).attr('y1', y).attr('x2', w).attr('y2', y)
        .attr('stroke', '#0b1628').attr('stroke-width', 1);

    const g = svg.append('g');
    svg.call(
      d3.zoom<SVGSVGElement, unknown>()
        .scaleExtent([0.25, 4])
        .on('zoom', ev => g.attr('transform', ev.transform))
    );

    // Force simülasyon
    this.simulation = d3.forceSimulation<WalletNode>(nodes)
      .force('link', d3.forceLink<WalletNode, TransactionEdge>(edges).id(d => d.id).distance(130))
      .force('charge', d3.forceManyBody().strength(-420))
      .force('center', d3.forceCenter(w / 2, h / 2))
      .force('collision', d3.forceCollide<WalletNode>().radius(d => nRadius(d.balance, allBal) + 14));

    // Kenarlar
    const eg = g.append('g');
    const epaths = eg.selectAll<SVGPathElement, TransactionEdge>('path')
      .data(edges).enter().append('path')
      .attr('fill', 'none')
      .attr('stroke', d => !hasHL ? C.ed : hlPath.edges.has(d.id) ? C.ehl : C.edim)
      .attr('stroke-width', d => eWidth(d.amount, allAmt))
      .attr('stroke-opacity', d => !hasHL ? 0.7 : hlPath.edges.has(d.id) ? 1 : 0.1)
      .attr('marker-end', d => !hasHL ? 'url(#arr-d)' : hlPath.edges.has(d.id) ? 'url(#arr-h)' : 'url(#arr-dim)')
      .style('cursor', 'pointer')
      .on('mouseover', (ev: MouseEvent, d) => {
        d3.select(ev.currentTarget as SVGPathElement).attr('stroke', C.acc).attr('stroke-opacity', 1);
        this.tooltip = {
          x: ev.offsetX + 14, y: ev.offsetY - 10,
          lines: [
            `TX: ${d.id}`,
  `Miktar: ${d.amount} ₿`,
  `${this.getNodeId(d.source)} → ${this.getNodeId(d.target)}`,
  `${new Date(d.timestamp).toLocaleString('tr-TR')}`,
          ],
        };
      })
      .on('mouseout', (ev: MouseEvent, d) => {
        const isHl = hlPath.edges.has(d.id);
        d3.select(ev.currentTarget as SVGPathElement)
          .attr('stroke', !hasHL ? C.ed : isHl ? C.ehl : C.edim)
          .attr('stroke-opacity', !hasHL ? 0.7 : isHl ? 1 : 0.1);
        this.tooltip = null;
      })
      .on('click', (ev: MouseEvent, d) => {
        ev.stopPropagation(); // Olayın alt katmanlara sızmasını engeller
        console.log('edge tıklandı:', d);

        this.selectedEdge = d;
        this.selectedNode = null;
        this.cdr.detectChanges();

        // ANA EKSİK BURASI:
        this.edgeClicked.emit(d);
      });

    // Kenar miktar etiketleri (Tıklanabilir Hitbox)
    const elabels = eg.selectAll<SVGTextElement, TransactionEdge>('text.el')
      .data(edges).enter().append('text').attr('class', 'el')
      .attr('text-anchor', 'middle').attr('dy', -4)
      .attr('fill', d => (!hasHL || hlPath.edges.has(d.id)) ? C.acc : 'transparent')
      .attr('font-size', 9).attr('font-family', "'Courier New',monospace")
      .style('cursor', 'pointer')
      .text(d => `${d.amount}₿`)
      .on('mouseover', (ev: MouseEvent, d) => {
        this.tooltip = {
          x: ev.offsetX + 14, y: ev.offsetY - 10,
          lines: [
            `TX: ${d.id}`,
            `Miktar: ${d.amount} ₿`,
            `${this.getNodeId(d.source)} → ${this.getNodeId(d.target)}`
          ],
        };
      })
      .on('mouseout', () => {
        this.tooltip = null;
      })
      .on('click', (ev: MouseEvent, d) => {
        ev.stopPropagation();
        console.log('Miktar yazısı üzerinden işlem tıklandı:', d);

        this.selectedEdge = d;
        this.selectedNode = null;
        this.cdr.detectChanges();

        this.edgeClicked.emit(d);
      });

    // Düğümler
    const ng2 = g.append('g');
    const ngrp = ng2.selectAll<SVGGElement, WalletNode>('g.n')
      .data(nodes).enter().append('g').attr('class', 'n')
      .style('cursor', 'pointer')
      .call(
        d3.drag<SVGGElement, WalletNode>()
          .on('start', (ev, d) => { if (!ev.active) this.simulation!.alphaTarget(0.3).restart(); d.fx = d.x; d.fy = d.y; })
          .on('drag', (ev, d) => { d.fx = ev.x; d.fy = ev.y; })
          .on('end', (ev, d) => { if (!ev.active) this.simulation!.alphaTarget(0); d.fx = null; d.fy = null; })
      );

    // Dış halka (highlight)
    ngrp.append('circle')
      .attr('r', d => nRadius(d.balance, allBal) + 7).attr('fill', 'none')
      .attr('stroke', d => d.label === 'BORSA' ? C.exc : hlPath.nodes.has(d.id) ? C.nhl : 'transparent')
      .attr('stroke-width', 1.5).attr('stroke-opacity', 0.5).attr('filter', 'url(#glow)');

    // Ana daire
    ngrp.append('circle').attr('class', 'nc')
      .attr('r', d => nRadius(d.balance, allBal))
      .attr('fill', d => {
        if (d.label === 'BORSA') return '#2d0808';
        return !hasHL ? C.nd : hlPath.nodes.has(d.id) ? '#152d15' : C.ndim;
      })
      .attr('stroke', d => {
        if (d.label === 'BORSA') return C.exc;
        return !hasHL ? C.nstr : hlPath.nodes.has(d.id) ? C.nhl : '#111e30';
      })
      .attr('stroke-width', d => hlPath.nodes.has(d.id) ? 2.5 : 1.5)
      .attr('filter', d => hlPath.nodes.has(d.id) ? 'url(#glow)' : 'none');

    // Bakiye etiketi
    ngrp.append('text').attr('text-anchor', 'middle').attr('dy', 4)
      .attr('fill', d => {
        if (d.label === 'BORSA') return '#f87171';
        return (!hasHL || hlPath.nodes.has(d.id)) ? C.acc : '#1a2a3a';
      })
      .attr('font-size', d => nRadius(d.balance, allBal) > 22 ? 9 : 7)
      .attr('font-family', "'Courier New',monospace").attr('font-weight', 'bold')
      .attr('pointer-events', 'none')
      .text(d => d.balance >= 1000 ? `${(d.balance / 1000).toFixed(1)}k` : String(d.balance));

    // Adres etiketi
    ngrp.append('text').attr('text-anchor', 'middle')
      .attr('dy', d => nRadius(d.balance, allBal) + 15)
      .attr('fill', d => (!hasHL || hlPath.nodes.has(d.id)) ? C.txt : '#0d1a28')
      .attr('font-size', 9).attr('font-family', "'Courier New',monospace")
      .attr('pointer-events', 'none')
      .text(d => d.label);

    // Hover + click
    ngrp
      .on('mouseover', (ev: MouseEvent, d) => {
        d3.select(ev.currentTarget as SVGGElement).select('.nc').attr('stroke-width', 3).attr('filter', 'url(#glow)');
        this.tooltip = {
          x: ev.offsetX + 14, y: ev.offsetY - 10,
          lines: [`Cüzdan: ${d.id}`, `Bakiye: ${d.balance} ₿`, `Tür: ${d.label === 'BORSA' ? 'Borsa' : 'Normal'}`],
        };
      })
      .on('mouseout', (ev: MouseEvent, d) => {
        const isHl = hlPath.nodes.has(d.id);
        d3.select(ev.currentTarget as SVGGElement).select('.nc')
          .attr('stroke-width', isHl ? 2.5 : 1.5)
          .attr('filter', isHl ? 'url(#glow)' : 'none');
        this.tooltip = null;
      })
      .on('click', (ev: MouseEvent, d) => {
        ev.stopPropagation(); // Boşluğa tıklanmış gibi algılanmasını önler
        this.selectedNode = d;
        this.selectedEdge = null;
        this.selectedNodeTxs = this.graphData.edges.filter(e => {
          const src = typeof e.source === 'object' ? (e.source as WalletNode).id : e.source;
          const tgt = typeof e.target === 'object' ? (e.target as WalletNode).id : e.target;
          return src === d.id || tgt === d.id;
        });
        this.cdr.detectChanges();

        this.nodeClicked.emit(d); // BİLGİYİ DIŞARI FIRLAT
      });

    // Tick: pozisyon güncelleme
    this.simulation.on('tick', () => {
      epaths.attr('d', d => {
        const src = d.source as WalletNode;
        const tgt = d.target as WalletNode;
        const r = nRadius(tgt.balance, allBal);
        const fwdKey = `${src.id}->${tgt.id}`;

        if (biMap.has(fwdKey)) {
          const dx = tgt.x! - src.x!, dy = tgt.y! - src.y!;
          const len = Math.sqrt(dx * dx + dy * dy) || 1;
          const cv = 42;
          const mx = (src.x! + tgt.x!) / 2 + (-dy / len) * cv;
          const my = (src.y! + tgt.y!) / 2 + (dx / len) * cv;
          const ang = Math.atan2(tgt.y! - my, tgt.x! - mx);
          return `M${src.x},${src.y} Q${mx},${my} ${tgt.x! - Math.cos(ang) * (r + 2)},${tgt.y! - Math.sin(ang) * (r + 2)}`;
        } else {
          const dx = tgt.x! - src.x!, dy = tgt.y! - src.y!;
          const len = Math.sqrt(dx * dx + dy * dy) || 1;
          return `M${src.x},${src.y} L${tgt.x! - (dx / len) * (r + 2)},${tgt.y! - (dy / len) * (r + 2)}`;
        }
      });

      elabels
        .attr('x', d => ((d.source as WalletNode).x! + (d.target as WalletNode).x!) / 2)
        .attr('y', d => ((d.source as WalletNode).y! + (d.target as WalletNode).y!) / 2);

      ngrp.attr('transform', d => `translate(${d.x},${d.y})`);
    });

    svg.on('click', () => this.clearSelection());
  }
}
