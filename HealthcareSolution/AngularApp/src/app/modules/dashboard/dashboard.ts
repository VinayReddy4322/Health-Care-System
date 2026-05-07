import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { DoctorService } from '../../services/doctor.service';
import { PatientService } from '../../services/patient.service';
import { AppointmentService } from '../../services/appointment.service';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.html'
})
export class Dashboard implements OnInit {
  doctors = 0;
  patients = 0;
  appointments = 0;
  message = '';

  constructor(
    public auth: AuthService,
    private doctorService: DoctorService,
    private patientService: PatientService,
    private appointmentService: AppointmentService
  ) {}

  ngOnInit(): void {
    this.doctorService.getAll().subscribe(doctors => this.doctors = doctors.length);
    this.appointmentService.getAll().subscribe({
      next: appointments => this.appointments = appointments.length,
      error: () => this.message = 'Could not load appointments.'
    });

    if (this.auth.currentUser()?.role === 'Admin') {
      this.patientService.getAll().subscribe(patients => this.patients = patients.length);
    }
  }
}
