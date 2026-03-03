import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UiPage } from './ui-page';

describe('UiPage', () => {
  let component: UiPage;
  let fixture: ComponentFixture<UiPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UiPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UiPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
