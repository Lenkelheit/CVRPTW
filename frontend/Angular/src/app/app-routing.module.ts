import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SolvingRoutingProblemComponent } from 'src/app/modules';

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
