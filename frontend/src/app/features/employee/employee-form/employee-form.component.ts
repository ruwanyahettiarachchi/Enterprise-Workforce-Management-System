import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTabsModule } from '@angular/material/tabs';
import { provideNativeDateAdapter } from '@angular/material/core';
import { EmployeeService } from '../../../core/services/employee.service';
import { DepartmentService } from '../../../core/services/department.service';
import { LocationService, SLCity } from '../../../core/services/location.service';
import { EmployeeStatus, Gender, MaritalStatus } from '../../../core/models/employee.model';
import { Department } from '../../../core/models/department.model';
import { NIC } from '@sri-lanka/nic';

// Custom Sri Lankan NIC validator using the installed npm library
export function nicValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) return null;
  return NIC.valid(value) ? null : { invalidNic: true };
}

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
    MatDatepickerModule,
    MatTabsModule
  ],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.scss'
})
export class EmployeeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private employeeService = inject(EmployeeService);
  private departmentService = inject(DepartmentService);
  private locationService = inject(LocationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private snackBar = inject(MatSnackBar);

  employeeForm!: FormGroup;
  isEditMode = false;
  employeeId: string | null = null;
  departments: Department[] = [];

  // SL Address API local bindings
  districts: string[] = [];
  cities: SLCity[] = [];
  province = '';

  // Enums for dropdown selections
  maritalStatuses = [
    { value: MaritalStatus.Single, label: 'Single' },
    { value: MaritalStatus.Married, label: 'Married' },
    { value: MaritalStatus.Divorced, label: 'Divorced' },
    { value: MaritalStatus.Widowed, label: 'Widowed' }
  ];

  genders = [
    { value: Gender.Male, label: 'Male' },
    { value: Gender.Female, label: 'Female' }
  ];

  activeTabIndex = 0;

  nextTab(): void {
    if (this.activeTabIndex < 2) {
      this.activeTabIndex++;
    }
  }

  previousTab(): void {
    if (this.activeTabIndex > 0) {
      this.activeTabIndex--;
    }
  }

  isTabValid(tabIndex: number): boolean {
    if (tabIndex === 0) {
      const fields = ['firstName', 'lastName', 'email', 'nic', 'maritalStatus'];
      return fields.every(f => {
        const ctrl = this.employeeForm.get(f);
        return ctrl ? ctrl.valid : false;
      });
    }
    if (tabIndex === 1) {
      const fields = ['addressLine1', 'district', 'city'];
      return fields.every(f => {
        const ctrl = this.employeeForm.get(f);
        return ctrl ? ctrl.valid : false;
      });
    }
    return true;
  }

  statuses = [
    { value: EmployeeStatus.Active, label: 'Active' },
    { value: EmployeeStatus.Probation, label: 'Probation' },
    { value: EmployeeStatus.Terminated, label: 'Terminated' }
  ];

  ngOnInit(): void {
    this.initForm();
    this.loadLocationDistricts();
    this.loadDepartments();
    this.setupListeners();
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
      departmentId: [null],
      
      // Demographic specifications
      nic: ['', [Validators.required, nicValidator]],
      dateOfBirth: [{ value: null, disabled: true }, [Validators.required]],
      gender: [{ value: null, disabled: true }, [Validators.required]],
      maritalStatus: [MaritalStatus.Single, [Validators.required]],
      
      // Address specifications
      addressLine1: ['', [Validators.required, Validators.maxLength(150)]],
      district: ['', [Validators.required]],
      city: ['', [Validators.required]],
      postalCode: [{ value: '', disabled: true }, [Validators.required]]
    });
  }

  private loadLocationDistricts(): void {
    this.locationService.getDistricts().subscribe({
      next: (data) => {
        this.districts = data;
      },
      error: (err) => console.error('Failed to load Sri Lankan districts data', err)
    });
  }

  private loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => console.error('Failed loading departments', err)
    });
  }

  private setupListeners(): void {
    // 1. NIC Value Change -> Auto-decodes gender and date of birth
    this.employeeForm.get('nic')?.valueChanges.subscribe(val => {
      if (val && NIC.valid(val)) {
        try {
          const parsed = NIC.parse(val);
          if (parsed && parsed.birthday) {
            // Parse custom birthday structure from library { year: number, month: number, day: number }
            const dob = new Date(parsed.birthday.year, parsed.birthday.month - 1, parsed.birthday.day);
            const genderVal = parsed.gender === 'MALE' ? Gender.Male : Gender.Female;

            this.employeeForm.patchValue({
              dateOfBirth: dob,
              gender: genderVal
            });
          }
        } catch (e) {
          console.error('Error decoding Sri Lankan NIC:', e);
        }
      }
    });

    // 2. District Selection Change -> Reload Cities & Auto-update Province
    this.employeeForm.get('district')?.valueChanges.subscribe(districtName => {
      if (districtName) {
        this.province = this.locationService.getProvinceForDistrict(districtName);
        this.locationService.getCitiesForDistrict(districtName).subscribe({
          next: (citiesList) => {
            this.cities = citiesList;
            
            // Clear current city selections if they aren't part of the new district
            const currentCity = this.employeeForm.get('city')?.value;
            if (currentCity && !citiesList.some(c => c.city === currentCity)) {
              this.employeeForm.patchValue({ city: '', postalCode: '' });
            }
          }
        });
      } else {
        this.cities = [];
        this.province = '';
        this.employeeForm.patchValue({ city: '', postalCode: '' });
      }
    });

    // 3. City Selection Change -> Auto-fills Postal Code
    this.employeeForm.get('city')?.valueChanges.subscribe(cityName => {
      if (cityName && this.cities.length) {
        const selectedCity = this.cities.find(c => c.city === cityName);
        if (selectedCity) {
          this.employeeForm.patchValue({ postalCode: selectedCity.code });
        }
      } else {
        this.employeeForm.patchValue({ postalCode: '' });
      }
    });
  }

  private checkEditMode(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id');
    if (this.employeeId) {
      this.isEditMode = true;
      this.employeeForm.get('nic')?.disable();
      this.employeeForm.get('joinDate')?.disable();

      this.employeeService.getEmployeeById(this.employeeId).subscribe({
        next: (employee) => {
          // Pre-load cities database first based on the employee's district
          if (employee.district) {
            this.locationService.getCitiesForDistrict(employee.district).subscribe({
              next: (citiesList) => {
                this.cities = citiesList;

                const joinDate = employee.joinDate ? new Date(employee.joinDate) : new Date();
                const dob = employee.dateOfBirth ? new Date(employee.dateOfBirth) : null;

                this.employeeForm.patchValue({
                  ...employee,
                  joinDate,
                  dateOfBirth: dob
                });
              }
            });
          }
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

    // Capture all form values, including disabled controls (NIC, DOB, Gender, PostalCode)
    const rawValue = this.employeeForm.getRawValue();
    
    // Format Date objects to YYYY-MM-DD strings for EF Core compatibility
    const joinDateObj = rawValue.joinDate as Date;
    const formattedJoinDate = joinDateObj.toISOString().split('T')[0];

    const dobObj = rawValue.dateOfBirth as Date;
    const formattedDOB = dobObj ? dobObj.toISOString().split('T')[0] : '';

    const employeePayload = {
      ...rawValue,
      joinDate: formattedJoinDate,
      dateOfBirth: formattedDOB
    };

    if (this.isEditMode && this.employeeId) {
      this.employeeService.updateEmployee(this.employeeId, employeePayload).subscribe({
        next: () => {
          this.snackBar.open('Employee profile updated successfully.', 'Close', { duration: 2000 });
          this.router.navigate(['/employees']);
        },
        error: (err) => {
          const errMsg = err.error?.message || 'Failed to update employee profile.';
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
          const errMsg = err.error?.message || 'Failed to register employee profile.';
          this.snackBar.open(errMsg, 'Close', { duration: 4000 });
          console.error(err);
        }
      });
    }
  }
}
