import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment'; // Adjust the path as needed

@Injectable({
  providedIn: 'root',
})
export class ClientService {
private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getClients(): Observable<any> {
    return this.http.get(this.baseUrl);
  }

  addClient(client: any): Observable<any> {
    return this.http.post(this.baseUrl, client);
  }

  updateClient(clientId: number, client: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${clientId}`, client);
  }

  deleteClient(clientId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${clientId}`);
  }
}
