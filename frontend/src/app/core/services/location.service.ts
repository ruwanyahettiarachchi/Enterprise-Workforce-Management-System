import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, of, tap } from 'rxjs';

export interface SLCity {
  city: string;
  code: string;
}

export interface SLData {
  [district: string]: SLCity[];
}

// Fixed Province mapping for Sri Lanka (25 districts to 9 provinces)
export const DISTRICT_PROVINCE_MAP: { [key: string]: string } = {
  'Ampara': 'Eastern', 'Batticaloa': 'Eastern', 'Trincomalee': 'Eastern',
  'Anuradhapura': 'North Central', 'Polonnaruwa': 'North Central',
  'Badulla': 'Uva', 'Moneragala': 'Uva',
  'Colombo': 'Western', 'Gampaha': 'Western', 'Kalutara': 'Western',
  'Galle': 'Southern', 'Hambantota': 'Southern', 'Matara': 'Southern',
  'Jaffna': 'Northern', 'Kilinochchi': 'Northern', 'Mannar': 'Northern', 'Mullaitivu': 'Northern', 'Vavuniya': 'Northern',
  'Kandy': 'Central', 'Matale': 'Central', 'Nuwara Eliya': 'Central',
  'Kegalle': 'Sabanagamuwa', 'Ratnapura': 'Sabanagamuwa',
  'Kurunegala': 'North Western', 'Puttalam': 'North Western'
};

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private http = inject(HttpClient);
  private cdnUrl = 'https://raw.githubusercontent.com/apsaraaruna/srilanka-cities-json/master/cities-and-postalcode-by-district-min.json';
  
  private cachedData: SLData | null = null;

  private loadData(): Observable<SLData> {
    if (this.cachedData) {
      return of(this.cachedData);
    }
    return this.http.get<SLData>(this.cdnUrl).pipe(
      tap(data => this.cachedData = data)
    );
  }

  getDistricts(): Observable<string[]> {
    return this.loadData().pipe(
      map(data => Object.keys(data).sort())
    );
  }

  getCitiesForDistrict(district: string): Observable<SLCity[]> {
    return this.loadData().pipe(
      map(data => {
        const cities = data[district] || [];
        return [...cities].sort((a, b) => a.city.localeCompare(b.city));
      })
    );
  }

  getProvinceForDistrict(district: string): string {
    return DISTRICT_PROVINCE_MAP[district] || 'Unknown';
  }
}
