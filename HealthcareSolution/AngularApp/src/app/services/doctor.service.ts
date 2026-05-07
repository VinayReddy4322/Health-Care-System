import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_BASE_URL } from './api.config';
import { Doctor, DoctorUpsert } from '../models/doctor.model';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<Doctor[]>(`${API_BASE_URL}/doctors`);
  }

  getMine() {
    return this.http.get<Doctor>(`${API_BASE_URL}/doctors/me`);
  }

  create(doctor: DoctorUpsert) {
    return this.http.post<Doctor>(`${API_BASE_URL}/doctors`, doctor);
  }

  update(id: number, doctor: DoctorUpsert) {
    return this.http.put<void>(`${API_BASE_URL}/doctors/${id}`, doctor);
  }

  delete(id: number) {
    return this.http.delete<void>(`${API_BASE_URL}/doctors/${id}`);
  }
}
