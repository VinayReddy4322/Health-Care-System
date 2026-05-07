export enum AppointmentStatus {
  Booked = 1,
  Cancelled = 2,
  Completed = 3
}

export interface Appointment {
  id: number;
  patientId: number;
  doctorId: number;
  appointmentDate: string;
  status: AppointmentStatus;
}

export interface AppointmentBooking {
  patientId: number;
  doctorId: number;
  appointmentDate: string;
}
