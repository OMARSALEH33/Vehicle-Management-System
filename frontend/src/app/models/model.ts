export interface KeyValuePair {
  id: number;
  name: string;
}

export interface Make {
  id: number;
  name: string;
  code: string;
}

export interface VehicleModel {
  id: number;
  code?: string;
  name: string;
  makeId: number;
}

export interface Feature {
  id: number;
  name: string;
}

export interface Contact {
  name: string;
  email?: string | null;
  phone: string;
}

export interface Vehicle {
  id: number;
  model: KeyValuePair;
  make: KeyValuePair;
  isRegistered: boolean;
  contact: Contact;
  lastUpdate: string;
  features: KeyValuePair[];
  
}
export interface ServiceResult<T> {
  status: number;
  data: T;
  field: string | null;
  error: string | null;
  isSuccess: boolean;
}

export interface SaveVehicle {
  code: string;
  makeName: string;
  modelName: string;
  isRegistered: boolean;
  contact: {
    name: string;
    email: string | null;
    phone: string;
  };
  features: number[];
}
export interface Model{
  id: number;
  name: string;
  makeId: number;
}
export interface AuthResponse{
  token: string;
}

export interface PagedResult<T>{
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

