import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopBarComponent } from './components/top-bar/top-bar.component';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { NotificationBarComponent } from './components/notification-bar/notification-bar.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
    declarations: [TopBarComponent, NotificationBarComponent],
    entryComponents: [NotificationBarComponent],
    imports: [
        CommonModule,
        RouterModule,
        MatToolbarModule,
        MatButtonModule,
        MatTableModule,
        MatSnackBarModule,
        MatIconModule
    ],
    exports: [
        TopBarComponent,
        NotificationBarComponent,
        RouterModule,
        MatToolbarModule,
        MatButtonModule,
        MatTableModule,
        MatSnackBarModule,
        MatIconModule
    ]
})
export class SharedModule { }
