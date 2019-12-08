import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
  <app-top-bar></app-top-bar>
  <router-outlet></router-outlet>`,
})
export class AppComponent {
  public title = 'Angular';
}
