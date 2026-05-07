import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { catchError, forkJoin, Observable, of } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { Appointment, AppointmentStatus } from '../../models/appointment.model';
import { Patient } from '../../models/patient.model';
import { AppointmentService } from '../../services/appointment.service';
import { DoctorService } from '../../services/doctor.service';
import { Doctor } from '../../models/doctor.model';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-doctor',
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor.html'
})
export class DoctorPage implements OnInit {
  readonly status = AppointmentStatus;
  doctor: Doctor | null = null;
  doctors: Doctor[] = [];
  appointments: Appointment[] = [];
  patientByUserId = new Map<number, Patient>();
  editingId: number | null = null;
  form = { userId: 0, name: '', specialization: '' };
  message = '';

  constructor(
    public auth: AuthService,
    private doctorService: DoctorService,
    private appointmentService: AppointmentService,
    private patientService: PatientService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.doctorService.getAll().subscribe(doctors => this.doctors = doctors);

    if (this.auth.currentUser()?.role === 'Doctor') {
      this.doctorService.getMine().subscribe({
        next: doctor => {
          this.doctor = doctor;
          this.form = { userId: doctor.userId, name: doctor.name, specialization: doctor.specialization };
        },
        error: () => this.message = 'Create your doctor profile.'
      });
    }

    if (this.auth.currentUser()?.role === 'Doctor' || this.auth.currentUser()?.role === 'Admin') {
      this.loadAppointments();
    }
  }

  save(): void {
    const userId = this.auth.currentUser()?.id ?? 0;
    const isAdmin = this.auth.currentUser()?.role === 'Admin';
    const body = { ...this.form, userId: isAdmin ? Number(this.form.userId) : userId };
    const targetId = isAdmin ? this.editingId : this.doctor?.id;
    const request: Observable<unknown> = targetId
      ? this.doctorService.update(targetId, body)
      : this.doctorService.create(body);

    request.subscribe({
      next: () => {
        this.message = 'Doctor profile saved.';
        this.resetForm();
        this.load();
      },
      error: (error: HttpErrorResponse) => this.message = typeof error.error === 'string' ? error.error : 'Could not save profile.'
    });
  }

  edit(doctor: Doctor): void {
    this.editingId = doctor.id;
    this.form = { userId: doctor.userId, name: doctor.name, specialization: doctor.specialization };
  }

  delete(doctor: Doctor): void {
    this.doctorService.delete(doctor.id).subscribe({
      next: () => {
        this.message = 'Doctor deleted.';
        if (this.editingId === doctor.id) this.resetForm();
        this.load();
      },
      error: (error: HttpErrorResponse) => this.message = typeof error.error === 'string' ? error.error : 'Could not delete doctor.'
    });
  }

  resetForm(): void {
    this.editingId = null;
    this.form = { userId: 0, name: '', specialization: '' };
  }

  patientName(userId: number): string {
    return this.patientByUserId.get(userId)?.name ?? `Patient #${userId}`;
  }

  statusLabel(status: AppointmentStatus): string {
    return this.appointmentService.statusLabel(status);
  }

  private loadAppointments(): void {
    this.appointmentService.getAll().subscribe({
      next: appointments => {
        this.appointments = appointments;
        this.loadPatientsForAppointments(appointments);
      },
      error: () => this.message = 'Could not load doctor appointments.'
    });
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
