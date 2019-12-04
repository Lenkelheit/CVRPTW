import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SolvingRoutingProblemComponent } from './components/solving-routing-problem/solving-routing-problem.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
    declarations: [SolvingRoutingProblemComponent],
    imports: [
        CommonModule,
        SharedModule
    ]
})
export class SolvingRoutingProblemModule { }
