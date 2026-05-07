import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_BASE_URL } from './api.config';
import { Appointment, AppointmentBooking, AppointmentStatus } from '../models/appointment.model';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<Appointment[]>(`${API_BASE_URL}/appointments`);
  }

  book(booking: AppointmentBooking) {
    return this.http.post<Appointment>(`${API_BASE_URL}/appointments/book`, booking);
  }

  cancel(id: number) {
    return this.http.post<void>(`${API_BASE_URL}/appointments/${id}/cancel`, {});
  }

  complete(id: number) {
    return this.http.post<void>(`${API_BASE_URL}/appointments/${id}/complete`, {});
  }

  statusLabel(status: AppointmentStatus): string {
    return status === AppointmentStatus.Booked
      ? 'Booked'
      : status === AppointmentStatus.Cancelled
        ? 'Cancelled'
        : 'Completed';
  }
}
