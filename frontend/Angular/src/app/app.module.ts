import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { SharedModule } from './shared/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SolvingRoutingProblemModule } from 'src/app/modules';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        SolvingRoutingProblemModule,
        AppRoutingModule,
        SharedModule,
        BrowserAnimationsModule,
        HttpClientModule
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
