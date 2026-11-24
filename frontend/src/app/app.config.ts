import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideToastr } from 'ngx-toastr';
import { authInterceptor } from './interceptors/authinterceptor';


export const appConfig: ApplicationConfig = {
  providers: [ 
    provideZoneChangeDetection({eventCoalescing: true}),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    provideToastr({
      timeOut: 3000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
      progressBar: true,
    }),
   provideHttpClient(withInterceptors([authInterceptor]))
    
  ]
}; 

export const API_URL = 'http://localhost:5165/api';
