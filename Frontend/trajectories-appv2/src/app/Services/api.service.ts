import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  apiUrl = "https://localhost:44371/api/trajectories"

  constructor(private http: HttpClient) {}

  coordinates = (filter: any) => {
    return this.http.post(`${this.apiUrl}/points`, filter)
  }

  trajectories = (filter: any) => {
    return this.http.post(`${this.apiUrl}`, filter)
  }

  trajectory = (id: string) => {
    return this.http.get(`${this.apiUrl}/${id}`)
  }

  aggregations = (filter: any) => {
    return this.http.post(`${this.apiUrl}/aggregation`, filter)
  }
}
