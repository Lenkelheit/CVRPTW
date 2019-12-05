import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { NotificationBarComponent } from 'src/app/shared/components/notification-bar/notification-bar.component';
import { FileOperationService } from 'src/app/services/file-operation.service';

@Component({
    selector: 'app-solving-routing-problem',
    templateUrl: './solving-routing-problem.component.html',
    styleUrls: ['./solving-routing-problem.component.less']
})
export class SolvingRoutingProblemComponent implements OnInit, OnDestroy {

    private hubConnection: HubConnection;

    public columnsToDisplay: string[] = ['id', 'name'];
    public vehicles = [];

    public ToDisplay: string[] = ['id', 'name'];
    public locations = [];

    public fileToUpload: File;
    public canDownload: boolean;

    constructor(public fileOperationService: FileOperationService, public snackBar: MatSnackBar) { }

    public ngOnInit() {
        this.registerHub();
    }

    private registerHub() {
        this.hubConnection = new HubConnectionBuilder().withUrl('http://localhost:5000/or-tools').build();
        this.hubConnection.start().catch((error) => {
            console.log('isSolved', error);

            this.openNotificationBar('Error occurred with connection to server.', 'notification-error');
        });

        this.hubConnection.on('IsSolved', (isSolved: boolean) => {
            this.canDownload = isSolved;
            this.openNotificationBar('Success! Results are ready for downloading.', 'notification-success');
            console.log('isSolved', isSolved);
        });
    }

    public ngOnDestroy(): void {
        this.hubConnection.stop();
    }

    public openFileInput() {
        document.getElementById('fileInput').click();
    }

    public selectFile(files: File[]) {
        this.fileToUpload = files[0];
    }

    public uploadFile() {
        this.fileOperationService.uploadFile(this.fileToUpload).subscribe(dataResp => {
            this.vehicles = dataResp.body.vehicles;
            this.locations = dataResp.body.locations;
            console.log(dataResp.body);
        },
            () =>
                this.openNotificationBar('Error occurred with uploading file.', 'notification-error')
        );
    }

    public downloadFile() {
        this.fileOperationService.downloadFile('blob').subscribe(fileResp => {
            this.saveAs(fileResp.body, 'Results.xlsx');
            this.canDownload = false;
        },
            () =>
                this.openNotificationBar('Error occurred with downloading file.', 'notification-error')
        );
    }

    private saveAs(blob: Blob, fileName: string) {
        const url = window.URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
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
