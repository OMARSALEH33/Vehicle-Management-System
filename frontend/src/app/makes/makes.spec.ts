import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Makes } from './makes';

describe('Makes', () => {
  let component: Makes;
  let fixture: ComponentFixture<Makes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Makes],
    }).compileComponents();

    fixture = TestBed.createComponent(Makes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
