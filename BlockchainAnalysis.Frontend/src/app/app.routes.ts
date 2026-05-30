import { Routes } from '@angular/router';
import { GraphEngineComponent } from './graph-engine/graph-engine';
import { BatuhanMerklePanelComponent } from './merkle-panel/merkle-panel';

export const routes: Routes = [
  { path: '', redirectTo: 'graph', pathMatch: 'full' },
  { path: 'graph', component: GraphEngineComponent },
  { path: 'merkle', component: BatuhanMerklePanelComponent },
];
