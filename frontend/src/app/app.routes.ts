import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Layout } from './layout/layout';
import { Dashboard } from './dashboard/dashboard';
import { Vehicles } from './vehicles/vehicles';
import { Setting } from './setting/setting';
import { Makes } from './makes/makes';
import { Models } from './models/models';
import { AddMakes } from './add-makes/add-makes';
import { adminGuard } from './guards/adminguard';
import { VehicleForm } from './vehicle-form/vehicle-form';
import { Forgotpassword } from './forgotpassword/forgotpassword';
import { ResetPassword } from './resetpassword/resetpassword';

export const routes: Routes = [
  
  { path:'login', component: Login },
  { path: 'forgot-password', component: Forgotpassword },
  { path: 'reset-password', component: ResetPassword },


  {
    path: '',
    component: Layout,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard},
      { path: 'vehicles', component: Vehicles},
      { path: 'vehicle-form', component: VehicleForm},
      { path: 'setting', component: Setting},
      { path: 'setting/makes', component: Makes},
      { path: 'setting/models', component: Models},
      { path: 'setting/makes/add-makes', component: AddMakes},
    ],
  },
  { path: '**', redirectTo:'dashboard'},
];
