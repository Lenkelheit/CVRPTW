import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// tslint:disable-next-line: max-line-length
import { SolvingRoutingProblemComponent } from './modules/solving-routing-problem/components/solving-routing-problem/solving-routing-problem.component';

const routes: Routes = [
    { path: '', redirectTo: 'solve', pathMatch: 'full' },
    { path: 'solve', component: SolvingRoutingProblemComponent },

    { path: '**', redirectTo: '' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
