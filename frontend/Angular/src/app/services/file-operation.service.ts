import { Injectable } from '@angular/core';
import { HttpInternalService } from './http-internal.service';

@Injectable({
    providedIn: 'root'
})
export class FileOperationService {
    private routePrefix = '/api/routing';

    constructor(private httpService: HttpInternalService) { }

    public uploadFile(fileToUpload: File) {
        const formData = new FormData();
        formData.append('file', fileToUpload, fileToUpload.name);
        return this.httpService.postRequest<any>(`${this.routePrefix}`, formData);
    }

    public downloadFile(responseType: string) {
        return this.httpService.getRequest<Blob>(`${this.routePrefix}`, null, responseType);
    }
}
