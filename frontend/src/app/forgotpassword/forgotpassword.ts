import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/authservice';
import { FormsModule } from '@angular/forms';

@Component({
  imports: [FormsModule],
  selector: 'app-forgotpassword',
  styleUrl: './forgotpassword.css',
  templateUrl: './forgotpassword.html',
})
export class Forgotpassword {
  private authService = inject(AuthService);
  private router = inject(Router);

  email: string = '';
  error: string = '';

  onSubmit() {
    this.error = '';

    this.authService.forgotPassword(this.email).subscribe({
      next: () => {
        this.router.navigate(['/reset-password'], {
          queryParams: { email: this.email },
        });
      },
      error: () => {
        this.error = 'Email not found or invalid request.';
      },
    });
  }
}