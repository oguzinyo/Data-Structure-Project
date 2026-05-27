import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MerklePanelComponent } from './merkle-panel/merkle-panel';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MerklePanelComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  title = 'BlockchainAnalysis.Frontend';
}
