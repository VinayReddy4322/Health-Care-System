import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { Auth } from './modules/auth/auth';
import { Dashboard } from './modules/dashboard/dashboard';
import { PatientPage } from './modules/patient/patient';
import { DoctorPage } from './modules/doctor/doctor';
import { AppointmentPage } from './modules/appointment/appointment';

export const routes: Routes = [
  { path: 'auth', component: Auth },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'patient', component: PatientPage, canActivate: [authGuard, roleGuard(['Patient', 'Admin'])] },
  { path: 'doctor', component: DoctorPage, canActivate: [authGuard, roleGuard(['Doctor', 'Admin'])] },
  { path: 'doctors', component: DoctorPage, canActivate: [authGuard] },
  { path: 'appointments', component: AppointmentPage, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: '**', redirectTo: 'dashboard' }
];
