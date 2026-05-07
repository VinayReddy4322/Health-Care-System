export interface Patient {
  id: number;
  userId: number;
  name: string;
  age: number;
}

export interface PatientUpsert {
  userId: number;
  name: string;
  age: number;
}
