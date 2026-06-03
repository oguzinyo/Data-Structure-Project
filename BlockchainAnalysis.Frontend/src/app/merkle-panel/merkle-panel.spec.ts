import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BatuhanMerklePanelComponent } from './merkle-panel';

describe('BatuhanMerklePanelComponent', () => {
  let component: BatuhanMerklePanelComponent;
  let fixture: ComponentFixture<BatuhanMerklePanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BatuhanMerklePanelComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BatuhanMerklePanelComponent);
    component = fixture.componentInstance;

    // Eğer ağaç verisi dışarıdan @Input ile bekleniyorsa, testin patlamaması için boş bir veri setliyoruz
    component.treeData = {
      rootHash: 'test',
      isValid: true,
      rootNode: { hash: 'test' }
    };

    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
