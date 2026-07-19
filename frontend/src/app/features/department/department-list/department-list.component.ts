import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { DepartmentService } from '../../../core/services/department.service';
import { EmployeeService } from '../../../core/services/employee.service';
import { Department } from '../../../core/models/department.model';
import { Employee } from '../../../core/models/employee.model';

@Component({
  selector: 'app-department-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule
  ],
  templateUrl: './department-list.component.html',
  styleUrl: './department-list.component.scss'
})
export class DepartmentListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private departmentService = inject(DepartmentService);
  private employeeService = inject(EmployeeService);
  private snackBar = inject(MatSnackBar);

  displayedColumns: string[] = ['name', 'code', 'manager', 'employees', 'actions'];
  departments: Department[] = [];
  employees: Employee[] = []; // Used for Manager selection dropdown
  
  departmentForm!: FormGroup;
  isEditMode = false;
  editingId: string | null = null;

  ngOnInit(): void {
    this.initForm();
    this.loadDepartments();
    this.loadEmployees();
  }

  private initForm(): void {
    this.departmentForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(10), Validators.pattern('^[a-zA-Z0-9]+$')]],
      managerId: [null]
    });
  }

  loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => {
        this.snackBar.open('Error loading departments. Check database connectivity.', 'Dismiss', { duration: 3000 });
        console.error(err);
      }
    });
  }

  loadEmployees(): void {
    // Load first 100 active employees to populate the Manager dropdown selection list
    this.employeeService.getEmployees(undefined, 0, 1, 100).subscribe({
      next: (result) => {
        this.employees = result.items;
      },
      error: (err) => console.error('Failed loading manager list options', err)
    });
  }

  selectDepartmentForEdit(department: Department): void {
    this.isEditMode = true;
    this.editingId = department.id;
    this.departmentForm.patchValue({
      name: department.name,
      code: department.code,
      managerId: department.managerId || null
    });
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.editingId = null;
    this.departmentForm.reset({
      name: '',
      code: '',
      managerId: null
    });
  }

  onSubmit(): void {
    if (this.departmentForm.invalid) {
      this.departmentForm.markAllAsTouched();
      return;
    }

    const payload = this.departmentForm.value;

    if (this.isEditMode && this.editingId) {
      this.departmentService.updateDepartment(this.editingId, payload).subscribe({
        next: () => {
          this.snackBar.open('Department updated successfully.', 'Close', { duration: 2000 });
          this.cancelEdit();
          this.loadDepartments();
        },
        error: (err) => {
          const msg = err.error?.message || 'Failed to update department.';
          this.snackBar.open(msg, 'Close', { duration: 4000 });
        }
      });
    } else {
      this.departmentService.createDepartment(payload).subscribe({
        next: () => {
          this.snackBar.open('Department created successfully.', 'Close', { duration: 2000 });
          this.departmentForm.reset({
            name: '',
            code: '',
            managerId: null
          });
          this.loadDepartments();
        },
        error: (err) => {
          const msg = err.error?.message || 'Failed to create department.';
          this.snackBar.open(msg, 'Close', { duration: 4000 });
        }
      });
    }
  }

  deleteDepartment(id: string): void {
    if (confirm('Are you sure you want to delete this department?')) {
      this.departmentService.deleteDepartment(id).subscribe({
        next: () => {
          this.snackBar.open('Department deleted successfully.', 'Close', { duration: 2000 });
          this.loadDepartments();
        },
        error: (err) => {
          // Deletion safety guard triggers a bad request if department is not empty
          const msg = err.error?.message || 'Cannot delete department.';
          this.snackBar.open(msg, 'Close', { duration: 4000 });
          console.error(err);
        }
      });
    }
  }
}
