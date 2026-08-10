import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats } from '../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private snackBar = inject(MatSnackBar);

  stats: DashboardStats | null = null;
  loading = true;

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading = true;
    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.snackBar.open('Error fetching dashboard metrics.', 'Dismiss', { duration: 3000 });
        console.error(err);
      }
    });
  }

  getMaxEmployeeCount(): number {
    if (!this.stats || !this.stats.departmentDistributions.length) return 1;
    return Math.max(...this.stats.departmentDistributions.map(d => d.employeeCount), 1);
  }

  getBarWidthPercentage(count: number): number {
    const max = this.getMaxEmployeeCount();
    return Math.round((count / max) * 100);
  }

  getMalePercentage(): number {
    if (!this.stats || !this.stats.genderBreakdown) return 0;
    const total = this.stats.genderBreakdown.maleCount + this.stats.genderBreakdown.femaleCount;
    if (total === 0) return 0;
    return Math.round((this.stats.genderBreakdown.maleCount / total) * 100);
  }

  getFemalePercentage(): number {
    if (!this.stats || !this.stats.genderBreakdown) return 0;
    const total = this.stats.genderBreakdown.maleCount + this.stats.genderBreakdown.femaleCount;
    if (total === 0) return 0;
    return Math.round((this.stats.genderBreakdown.femaleCount / total) * 100);
  }

  getMaritalPercentage(count: number): number {
    if (!this.stats || !this.stats.maritalStatusBreakdowns) return 0;
    const total = this.stats.maritalStatusBreakdowns.reduce((sum, item) => sum + item.count, 0);
    if (total === 0) return 0;
    return Math.round((count / total) * 100);
  }
}
