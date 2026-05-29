import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GraphEngineComponent } from './graph-engine';

describe('GraphEngine', () => {
  let component: GraphEngineComponent;
  let fixture: ComponentFixture<GraphEngineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GraphEngineComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GraphEngineComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
