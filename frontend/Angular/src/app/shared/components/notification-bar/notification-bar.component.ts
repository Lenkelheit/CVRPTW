import { Component } from '@angular/core';
import { MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
    selector: 'app-notification-bar',
    templateUrl: './notification-bar.component.html',
    styleUrls: ['./notification-bar.component.less']
})
export class NotificationBarComponent {

    constructor(public snackBarRef: MatSnackBarRef<NotificationBarComponent>) { }
}
