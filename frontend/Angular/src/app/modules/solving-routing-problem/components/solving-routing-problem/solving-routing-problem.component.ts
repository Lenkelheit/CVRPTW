import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationBarComponent } from 'src/app/shared/components/notification-bar/notification-bar.component';
import { FileOperationService } from 'src/app/services/file-operation.service';

@Component({
    selector: 'app-solving-routing-problem',
    templateUrl: './solving-routing-problem.component.html',
    styleUrls: ['./solving-routing-problem.component.less']
})
export class SolvingRoutingProblemComponent implements OnInit {
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

    constructor(public fileOperationService: FileOperationService, public snackBar: MatSnackBar) { }

    public ngOnInit() {
        this.openNotificationBar();
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
