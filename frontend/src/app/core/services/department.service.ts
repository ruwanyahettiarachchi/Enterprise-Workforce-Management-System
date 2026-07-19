import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from '../models/department.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/departments';

  getDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(this.apiUrl);
  }

  getDepartmentById(id: string): Observable<Department> {
    return this.http.get<Department>(`${this.apiUrl}/${id}`);
  }

  createDepartment(department: Partial<Department>): Observable<Department> {
    return this.http.post<Department>(this.apiUrl, department);
  }

  updateDepartment(id: string, department: Partial<Department>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, department);
  }

  deleteDepartment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
