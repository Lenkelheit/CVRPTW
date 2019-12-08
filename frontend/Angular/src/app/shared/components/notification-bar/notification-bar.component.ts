import { Component, Inject } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

@Component({
    selector: 'app-notification-bar',
    templateUrl: './notification-bar.component.html',
    styleUrls: ['./notification-bar.component.less']
})
export class NotificationBarComponent {

    constructor(public snackBarRef: MatSnackBarRef<NotificationBarComponent>, @Inject(MAT_SNACK_BAR_DATA) public data: any) { }
}
