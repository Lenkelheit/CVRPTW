import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { SolvingRoutingProblemComponent } from './components/solving-routing-problem/solving-routing-problem.component';

@NgModule({
    declarations: [SolvingRoutingProblemComponent],
    imports: [
        CommonModule,
        SharedModule
    ]
})
export class SolvingRoutingProblemModule { }
