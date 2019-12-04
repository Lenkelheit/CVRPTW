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

    public columnsToDisplay: string[] = ['vehicleName', 'orderName', 'locationName'];
    public data = [
        {
            vehicleName: 'Vehicle 1',
            orderName: 'Order 1',
            locationName: 'Store 1'
        },
        {
            vehicleName: 'Vehicle 2',
            orderName: 'Order 2',
            locationName: 'Store 2'
        },
        {
            vehicleName: 'Vehicle 3',
            orderName: 'Order 3',
            locationName: 'Store 3'
        },
        {
            vehicleName: 'Vehicle 3',
            orderName: 'Order 3',
            locationName: 'Store 3'
        }
    ];
    public fileToUpload: File;
    public canDownload: boolean;

    constructor(public fileOperationService: FileOperationService, public snackBar: MatSnackBar) { }

    public ngOnInit() {
        this.openNotificationBar();
        this.registerHub();
    }

    private registerHub() {
        this.hubConnection = new HubConnectionBuilder().withUrl('http://localhost:5000/or-tools').build();
        this.hubConnection.start().catch((error) =>
        console.log('isSolved', error));

        this.hubConnection.on('IsSolved', (isSolved: boolean) => {
            this.canDownload = isSolved;
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
            console.log(dataResp.body);
        });
    }

    public downloadFile() {
        this.fileOperationService.downloadFile('blob').subscribe(fileResp => {
            this.saveAs(fileResp.body, 'Results.xlsx');
            this.canDownload = false;
        });
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

    private openNotificationBar() {
        this.snackBar.openFromComponent(NotificationBarComponent, {
            duration: 10000,
            panelClass: ['notification-success'],
            horizontalPosition: 'end'
        });
    }
}
