import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest, UserRole } from '../../models/user.model';

@Component({
  selector: 'app-auth',
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.html'
})
export class Auth {
  loginForm = { username: 'admin', password: 'admin123' };
  registerForm: RegisterRequest = { username: '', password: '', role: 'Patient' };
  roles: UserRole[] = ['Patient', 'Doctor', 'Admin'];
  message = '';
  busy = false;

  constructor(private auth: AuthService, private router: Router) {}

  login(): void {
    this.busy = true;
    this.auth.login(this.loginForm).subscribe({
      next: () => this.router.navigateByUrl('/dashboard'),
      error: error => this.fail(error, 'Login failed'),
      complete: () => this.busy = false
    });
  }

  register(): void {
    this.busy = true;
    this.auth.register(this.registerForm).subscribe({
      next: () => {
        this.message = 'User registered. Sign in with the new account.';
        this.registerForm = { username: '', password: '', role: 'Patient' };
      },
      error: error => this.fail(error, 'Registration failed'),
      complete: () => this.busy = false
    });
  }

  private fail(error: any, fallback: string): void {
    this.busy = false;
    this.message = typeof error?.error === 'string' ? error.error : fallback;
  }
}
