import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { EmployeeService } from '../../../core/services/employee.service';
import { DepartmentService } from '../../../core/services/department.service';
import { EmployeeStatus } from '../../../core/models/employee.model';
import { Department } from '../../../core/models/department.model';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatDatepickerModule
  ],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.scss'
})
export class EmployeeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private employeeService = inject(EmployeeService);
  private departmentService = inject(DepartmentService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private snackBar = inject(MatSnackBar);

  employeeForm!: FormGroup;
  isEditMode = false;
  employeeId: string | null = null;
  departments: Department[] = [];

  ngOnInit(): void {
    this.initForm();
    this.loadDepartments();
    this.checkEditMode();
  }

  private initForm(): void {
    this.employeeForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      phone: ['', [Validators.maxLength(20)]],
      jobTitle: ['', [Validators.required, Validators.maxLength(100)]],
      joinDate: [new Date(), [Validators.required]],
      status: [EmployeeStatus.Active, [Validators.required]],
      departmentId: [null]
    });
  }

  private loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => {
        console.error('Failed loading departments menu options', err);
      }
    });
  }

  private checkEditMode(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id');
    if (this.employeeId) {
      this.isEditMode = true;
      this.employeeService.getEmployeeById(this.employeeId).subscribe({
        next: (employee) => {
          const joinDate = employee.joinDate ? new Date(employee.joinDate) : new Date();
          this.employeeForm.patchValue({
            ...employee,
            joinDate
          });
        },
        error: (err) => {
          this.snackBar.open('Error loading employee record.', 'Dismiss', { duration: 3000 });
          console.error(err);
          this.router.navigate(['/employees']);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    const formValue = this.employeeForm.value;
    
    // Format joinDate to YYYY-MM-DD for EF Core mapping compatibility
    const dateObj = formValue.joinDate as Date;
    const formattedJoinDate = dateObj.toISOString().split('T')[0];

    const employeePayload = {
      ...formValue,
      joinDate: formattedJoinDate
    };

    if (this.isEditMode && this.employeeId) {
      this.employeeService.updateEmployee(this.employeeId, employeePayload).subscribe({
        next: () => {
          this.snackBar.open('Employee record updated successfully.', 'Close', { duration: 2000 });
          this.router.navigate(['/employees']);
        },
        error: (err) => {
          const errMsg = err.error?.message || 'Failed to update employee.';
          this.snackBar.open(errMsg, 'Close', { duration: 4000 });
          console.error(err);
        }
      });
    } else {
      this.employeeService.createEmployee(employeePayload).subscribe({
        next: () => {
          this.snackBar.open('Employee record registered successfully.', 'Close', { duration: 2000 });
          this.router.navigate(['/employees']);
        },
        error: (err) => {
          const errMsg = err.error?.message || 'Failed to register employee.';
          this.snackBar.open(errMsg, 'Close', { duration: 4000 });
          console.error(err);
        }
      });
    }
  }
}
