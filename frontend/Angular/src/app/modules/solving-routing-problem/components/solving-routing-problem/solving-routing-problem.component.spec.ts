import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SolvingRoutingProblemComponent } from './solving-routing-problem.component';

describe('SolvingRoutingProblemComponent', () => {
  let component: SolvingRoutingProblemComponent;
  let fixture: ComponentFixture<SolvingRoutingProblemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SolvingRoutingProblemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SolvingRoutingProblemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
