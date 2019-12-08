import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';

import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { NotificationBarComponent } from 'src/app/shared/components/notification-bar/notification-bar.component';
import { FileOperationService } from 'src/app/services/file-operation.service';
import { environment as API } from 'src/environments/environment';

@Component({
    selector: 'app-solving-routing-problem',
    templateUrl: './solving-routing-problem.component.html',
    styleUrls: ['./solving-routing-problem.component.less']
})
export class SolvingRoutingProblemComponent implements OnInit, OnDestroy {

    public vehiclesColumns: string[] = ['id', 'name'];
    public vehicles = [];

    public locationsColumns: string[] = ['id', 'name'];
    public locations = [];

    public fileToUpload: File;
    public canDownload: boolean;

    private $unsubscribe = new Subject<void>();
    private hubConnection: HubConnection;
    constructor(
        private fileOperationService: FileOperationService,
        private snackBar: MatSnackBar) { }

    public ngOnInit(): void {
        this.registerHub();
    }

    private registerHub(): void {
        this.hubConnection = new HubConnectionBuilder().withUrl(`${API.apiUrl}/${API.orToolsHub}`).build();
        this.hubConnection.start().catch(() =>
            this.openNotificationBar('Error occurred with connection to server.', 'notification-error')
        );

        this.hubConnection.on('IsSolved', this.isOperationSolved.bind(this));
    }

    public ngOnDestroy(): void {
        this.hubConnection.stop();

        this.$unsubscribe.next();
        this.$unsubscribe.complete();
    }

    public isOperationSolved(isSolved: boolean) {
        this.canDownload = isSolved;

        if (isSolved) this.openNotificationBar('Success! Results are ready for downloading.', 'notification-success');
        else          this.openNotificationBar('Solution is not found.', 'notification-error');
    }

    public openFileInput() {
        document.getElementById('fileInput').click();
    }

    public selectFile(files: File[]) {
        this.fileToUpload = files[0];
    }

    public uploadFile() {
        this
        .fileOperationService
        .uploadFile(this.fileToUpload)
        .pipe(takeUntil(this.$unsubscribe))
        .subscribe(
            dataResp => {
                this.vehicles = dataResp.body.vehicles;
                this.locations = dataResp.body.locations;
            },
            () =>
                this.openNotificationBar('Error occurred with uploading file.', 'notification-error'));
    }

    public downloadFile() {
        this
        .fileOperationService
        .downloadFile('blob')
        .pipe(takeUntil(this.$unsubscribe))
        .subscribe(
            fileResp => {
                this.saveAs(fileResp.body, 'Results.xlsx');
                this.canDownload = false;
            },
            () =>
                this.openNotificationBar('Error occurred with downloading file.', 'notification-error'));
    }

    private saveAs(blob: Blob, fileName: string) {
        const url = window.URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        link.click();
    }

    private openNotificationBar(message: string, panelClass: string) {
        this.snackBar.openFromComponent(NotificationBarComponent, {
            data: message,
            duration: 10000,
            panelClass: [panelClass],
            horizontalPosition: 'end'
        });
    }
}
