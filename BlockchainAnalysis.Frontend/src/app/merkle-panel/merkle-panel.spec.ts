import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MerklePanel } from './merkle-panel';

describe('MerklePanel', () => {
  let component: MerklePanel;
  let fixture: ComponentFixture<MerklePanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MerklePanel],
    }).compileComponents();

    fixture = TestBed.createComponent(MerklePanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
