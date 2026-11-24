import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddModel } from './add-model';

describe('AddModel', () => {
  let component: AddModel;
  let fixture: ComponentFixture<AddModel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddModel],
    }).compileComponents();

    fixture = TestBed.createComponent(AddModel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
