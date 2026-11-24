import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/authservice';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  private readonly router = inject(Router);
  private readonly AuthService = inject(AuthService);

  username = signal('');
  password = signal('');
  showPassword = signal(false);
  submitted = signal(false);
  erroMessage = signal('');

  
  onSubmit(event: Event) {
    event.preventDefault();
    this.submitted.set(true);

    if (this.isFormInvalid()) {
      return;
    }

    const payload = {
      username: this.username(),
      password: this.password(),
    };

    this.AuthService.login(payload).subscribe({
      next: (res) => {
        console.log('Login Success',res);
        this.router.navigate(['/dashboard']);

      },
      error: (err) => {
        console.error(err);
        this.erroMessage.set('Username and password incorrect');
      }
    });
  }
 
  private isFormInvalid(): boolean {
    const isUsernameEmpty = !this.username().trim();
    const isPasswordEmpty = !this.password();

    return isUsernameEmpty || isPasswordEmpty;
  }
}