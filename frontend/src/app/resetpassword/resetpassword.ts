import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../services/authservice';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './resetpassword.html',
  styleUrls: ['./resetpassword.css'],
})
export class ResetPassword implements OnInit {
  email = '';
  newPassword = '';
  message = '';
  error = '';

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'] || '';
    });
  }

  onSubmit() {
    this.error = '';
    this.message = '';

    if (!this.email) {
      this.error = 'Email is missing. Please start from Forgot Password.';
      return;
    }

    const model = {
      email: this.email,
      password: this.newPassword,
    };

    this.authService.resetPassword(model).subscribe({
      next: () => {
        this.message = 'Password has been reset successfully! Redirecting to login...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: () => {
        this.error = 'Failed to reset password. Please try again.';
      },
    });
  }
}