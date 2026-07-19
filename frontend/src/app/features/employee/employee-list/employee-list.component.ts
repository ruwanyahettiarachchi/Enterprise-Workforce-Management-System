import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { EmployeeService } from '../../../core/services/employee.service';
import { Employee, EmployeeStatus } from '../../../core/models/employee.model';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule
  ],
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.scss'
})
export class EmployeeListComponent implements OnInit {
  private employeeService = inject(EmployeeService);
  private snackBar = inject(MatSnackBar);

  displayedColumns: string[] = ['name', 'email', 'jobTitle', 'department', 'status', 'joinDate', 'actions'];
  employees: Employee[] = [];
  
  // Pagination & Filtering state
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  
  searchTerm = '';
  statusFilter: number | undefined = undefined;

  // Search debounce
  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    this.loadEmployees();

    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => {
      this.searchTerm = value;
      this.pageIndex = 0; // reset to first page
      this.loadEmployees();
    });
  }

  loadEmployees(): void {
    this.employeeService.getEmployees(
      this.searchTerm,
      this.statusFilter,
      this.pageIndex + 1,
      this.pageSize
    ).subscribe({
      next: (result) => {
        this.employees = result.items;
        this.totalCount = result.totalCount;
      },
      error: (err) => {
        this.snackBar.open('Error loading employee directory. Check API connection.', 'Dismiss', {
          duration: 3000
        });
        console.error(err);
      }
    });
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  onStatusFilterChange(value: number | string): void {
    this.statusFilter = value === 'all' ? undefined : Number(value);
    this.pageIndex = 0;
    this.loadEmployees();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadEmployees();
  }

  deleteEmployee(id: string): void {
    if (confirm('Are you sure you want to delete this employee? This action cannot be undone.')) {
      this.employeeService.deleteEmployee(id).subscribe({
        next: () => {
          this.snackBar.open('Employee record deleted successfully.', 'Close', { duration: 2000 });
          this.loadEmployees();
        },
        error: (err) => {
          this.snackBar.open('Failed to delete employee record.', 'Close', { duration: 3000 });
          console.error(err);
        }
      });
    }
  }

  getStatusLabel(status: EmployeeStatus): string {
    switch (status) {
      case EmployeeStatus.Active: return 'Active';
      case EmployeeStatus.Probation: return 'Probation';
      case EmployeeStatus.Terminated: return 'Terminated';
      default: return 'Unknown';
    }
  }

  getStatusClass(status: EmployeeStatus): string {
    switch (status) {
      case EmployeeStatus.Active: return 'status-active';
      case EmployeeStatus.Probation: return 'status-probation';
      case EmployeeStatus.Terminated: return 'status-terminated';
      default: return '';
    }
  }
}
