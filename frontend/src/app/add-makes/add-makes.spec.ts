import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddMakes } from './add-makes';

describe('AddMakes', () => {
  let component: AddMakes;
  let fixture: ComponentFixture<AddMakes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddMakes],
    }).compileComponents();

    fixture = TestBed.createComponent(AddMakes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
