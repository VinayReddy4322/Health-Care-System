import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { PatientService } from '../../services/patient.service';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-patient',
  imports: [CommonModule, FormsModule],
  templateUrl: './patient.html'
})
export class PatientPage implements OnInit {
  patient: Patient | null = null;
  patients: Patient[] = [];
  editingId: number | null = null;
  form = { userId: 0, name: '', age: 30 };
  message = '';

  constructor(public auth: AuthService, private patientService: PatientService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    if (this.auth.currentUser()?.role === 'Admin') {
      this.patientService.getAll().subscribe(patients => this.patients = patients);
      return;
    }

    this.patientService.getMine().subscribe({
      next: patient => {
        this.patient = patient;
        this.form = { userId: patient.userId, name: patient.name, age: patient.age };
      },
      error: () => this.message = 'Create your patient profile.'
    });
  }

  save(): void {
    const userId = this.auth.currentUser()?.id ?? 0;
    const isAdmin = this.auth.currentUser()?.role === 'Admin';
    const body = { ...this.form, userId: isAdmin ? Number(this.form.userId) : userId };
    const targetId = isAdmin ? this.editingId : this.patient?.id;
    const request: Observable<unknown> = targetId
      ? this.patientService.update(targetId, body)
      : this.patientService.create(body);

    request.subscribe({
      next: () => {
        this.message = 'Patient profile saved.';
        this.resetForm();
        this.load();
      },
      error: (error: HttpErrorResponse) => this.message = typeof error.error === 'string' ? error.error : 'Could not save profile.'
    });
  }

  edit(patient: Patient): void {
    this.editingId = patient.id;
    this.form = { userId: patient.userId, name: patient.name, age: patient.age };
  }

  delete(patient: Patient): void {
    this.patientService.delete(patient.id).subscribe({
      next: () => {
        this.message = 'Patient deleted.';
        if (this.editingId === patient.id) this.resetForm();
        this.load();
      },
      error: (error: HttpErrorResponse) => this.message = typeof error.error === 'string' ? error.error : 'Could not delete patient.'
    });
  }

  resetForm(): void {
    this.editingId = null;
    this.form = { userId: 0, name: '', age: 30 };
  }
}
