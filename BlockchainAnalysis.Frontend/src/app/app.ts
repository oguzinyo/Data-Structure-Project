import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {  RouterLink } from '@angular/router';
import { MerklePanelComponent } from './merkle-panel/merkle-panel';
import { GraphEngineComponent } from './graph-engine/graph-engine';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,RouterOutlet, RouterLink],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  title = 'BlockchainAnalysis.Frontend';
}
