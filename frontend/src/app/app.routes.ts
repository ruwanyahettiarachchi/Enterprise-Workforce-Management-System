import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component')
      .then(m => m.DashboardComponent)
  },
  {
    path: 'employees',
    loadComponent: () => import('./features/employee/employee-list/employee-list.component')
      .then(m => m.EmployeeListComponent)
  },
  {
    path: 'employees/new',
    loadComponent: () => import('./features/employee/employee-form/employee-form.component')
      .then(m => m.EmployeeFormComponent)
  },
  {
    path: 'employees/edit/:id',
    loadComponent: () => import('./features/employee/employee-form/employee-form.component')
      .then(m => m.EmployeeFormComponent)
  },
  {
    path: 'departments',
    loadComponent: () => import('./features/department/department-list/department-list.component')
      .then(m => m.DepartmentListComponent)
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
