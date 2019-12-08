import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class HttpInternalService {
    private baseUrl: string = environment.apiUrl;

    constructor(private http: HttpClient) { }

    public getRequest<T>(url: string, httpParams?: any, responseType?: any): Observable<HttpResponse<T>> {
        return this.http.get<T>(this.baseUrl + url, {
            observe: 'response',
            params: httpParams,
            responseType
        });
    }

    public postRequest<T>(url: string, payload: object): Observable<HttpResponse<T>> {
        return this.http.post<T>(this.baseUrl + url, payload, {
            observe: 'response'
        });
    }
}
