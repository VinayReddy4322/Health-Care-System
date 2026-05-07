export interface Doctor {
  id: number;
  userId: number;
  name: string;
  specialization: string;
}

export interface DoctorUpsert {
  userId: number;
  name: string;
  specialization: string;
}
