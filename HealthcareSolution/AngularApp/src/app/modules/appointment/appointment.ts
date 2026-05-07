import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { catchError, forkJoin, of } from 'rxjs';
import { Appointment, AppointmentStatus } from '../../models/appointment.model';
import { Doctor } from '../../models/doctor.model';
import { Patient } from '../../models/patient.model';
import { AuthService } from '../../services/auth.service';
import { AppointmentService } from '../../services/appointment.service';
import { DoctorService } from '../../services/doctor.service';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-appointment',
  imports: [CommonModule, FormsModule],
  templateUrl: './appointment.html'
})
export class AppointmentPage implements OnInit {
  readonly status = AppointmentStatus;
  doctors: Doctor[] = [];
  patientByUserId = new Map<number, Patient>();
  appointments: Appointment[] = [];
  booking = { doctorId: 0, appointmentDate: this.tomorrowLocal() };
  message = '';

  constructor(
    public auth: AuthService,
    private appointmentService: AppointmentService,
    private doctorService: DoctorService,
    private patientService: PatientService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.doctorService.getAll().subscribe(doctors => {
      this.doctors = doctors;
      this.booking.doctorId = this.booking.doctorId || doctors[0]?.userId || 0;
    });

    this.appointmentService.getAll().subscribe({
      next: appointments => {
        this.appointments = appointments;
        this.loadPatientsForAppointments(appointments);
      },
      error: () => this.message = 'Could not load appointments.'
    });
  }

  book(): void {
    const userId = this.auth.currentUser()?.id ?? 0;
    this.appointmentService.book({
      patientId: userId,
      doctorId: Number(this.booking.doctorId),
      appointmentDate: new Date(this.booking.appointmentDate).toISOString()
    }).subscribe({
      next: () => {
        this.message = 'Appointment booked.';
        this.load();
      },
      error: error => this.message = typeof error?.error === 'string' ? error.error : 'Could not book appointment.'
    });
  }

  cancel(id: number): void {
    this.appointmentService.cancel(id).subscribe({
      next: () => {
        this.message = 'Appointment cancelled.';
        this.load();
      },
      error: error => this.message = typeof error?.error === 'string' ? error.error : 'Could not cancel appointment.'
    });
  }

  complete(id: number): void {
    this.appointmentService.complete(id).subscribe({
      next: () => {
        this.message = 'Appointment completed.';
        this.load();
      },
      error: error => this.message = typeof error?.error === 'string' ? error.error : 'Could not complete appointment.'
    });
  }

  doctorName(id: number): string {
    return this.doctors.find(doctor => doctor.userId === id || doctor.id === id)?.name ?? `Doctor #${id}`;
  }

  patientName(id: number): string {
    return this.patientByUserId.get(id)?.name ?? `Patient #${id}`;
  }

  statusLabel(status: AppointmentStatus): string {
    return this.appointmentService.statusLabel(status);
  }

  canBook(): boolean {
    const role = this.auth.currentUser()?.role;
    return role === 'Patient' || role === 'Admin';
  }

  canComplete(appointment: Appointment): boolean {
    const role = this.auth.currentUser()?.role;
    return appointment.status === AppointmentStatus.Booked && (role === 'Doctor' || role === 'Admin');
  }

  canCancel(appointment: Appointment): boolean {
    return appointment.status === AppointmentStatus.Booked;
  }

  private tomorrowLocal(): string {
    const date = new Date();
    date.setDate(date.getDate() + 1);
    date.setHours(10, 0, 0, 0);
    return date.toISOString().slice(0, 16);
  }

  private loadPatientsForAppointments(appointments: Appointment[]): void {
    const patientIds = [...new Set(appointments.map(appointment => appointment.patientId))];
    if (patientIds.length === 0) {
      this.patientByUserId.clear();
      return;
    }

    forkJoin(patientIds.map(userId =>
      this.patientService.getByUserId(userId).pipe(catchError(() => of(null)))
    )).subscribe(patients => {
      this.patientByUserId = new Map(
        patients.filter((patient): patient is Patient => !!patient).map(patient => [patient.userId, patient])
      );
    });
  }
}
