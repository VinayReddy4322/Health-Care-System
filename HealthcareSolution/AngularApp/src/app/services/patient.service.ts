import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, forkJoin, map, of } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { Patient, PatientUpsert } from '../models/patient.model';

@Injectable({ providedIn: 'root' })
export class PatientService {
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<Patient[]>(`${API_BASE_URL}/patients`);
  }

  getMine() {
    return this.http.get<Patient>(`${API_BASE_URL}/patients/me`);
  }

  getByUserId(userId: number) {
    return this.http.get<Patient>(`${API_BASE_URL}/patients/by-user/${userId}`);
  }

  getByUserIds(userIds: number[]) {
    const uniqueIds = [...new Set(userIds)].filter(id => id > 0);
    if (uniqueIds.length === 0) return of(new Map<number, Patient>());

    return forkJoin(
      uniqueIds.map(userId => this.getByUserId(userId).pipe(catchError(() => of(null))))
    ).pipe(
      mapPatientsByUserId()
    );
  }

  create(patient: PatientUpsert) {
    return this.http.post<Patient>(`${API_BASE_URL}/patients`, patient);
  }

  update(id: number, patient: PatientUpsert) {
    return this.http.put<void>(`${API_BASE_URL}/patients/${id}`, patient);
  }

  delete(id: number) {
    return this.http.delete<void>(`${API_BASE_URL}/patients/${id}`);
  }
}

function mapPatientsByUserId() {
  return map((patients: (Patient | null)[]) =>
    new Map(
      patients
        .filter((patient): patient is Patient => !!patient)
        .map(patient => [patient.userId, patient])
    )
  );
}
