import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Headercomponent } from './headercomponent';

describe('Headercomponent', () => {
  let component: Headercomponent;
  let fixture: ComponentFixture<Headercomponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Headercomponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Headercomponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
