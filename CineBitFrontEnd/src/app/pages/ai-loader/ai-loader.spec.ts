import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AiLoader } from './ai-loader';

describe('AiLoader', () => {
  let component: AiLoader;
  let fixture: ComponentFixture<AiLoader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AiLoader]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AiLoader);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
