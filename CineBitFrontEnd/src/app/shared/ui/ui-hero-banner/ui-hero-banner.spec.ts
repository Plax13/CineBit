import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UiHeroBanner } from './ui-hero-banner';

describe('UiHeroBanner', () => {
  let component: UiHeroBanner;
  let fixture: ComponentFixture<UiHeroBanner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UiHeroBanner]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UiHeroBanner);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
