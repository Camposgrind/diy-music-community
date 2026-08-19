import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthApiService } from '../../../infrastructure/api/auth-api.service';
import { ToastService } from '../../../core/toast/toast.service';
import { ApiError } from '../../../infrastructure/api/models';
import { passwordMatchValidator } from './password-match.validator';
import { AuthService } from '../../../core/auth/auth.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'dmc-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly authApi = inject(AuthApiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly loading = signal(false);
  readonly showPassword = signal(false);
  readonly showConfirm = signal(false);

  readonly form = new FormGroup(
    {
      username: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        Validators.pattern(/^[a-zA-Z0-9_-]+$/),
      ]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.maxLength(256),
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$/),
      ]),
      confirmPassword: new FormControl('', [Validators.required]),
    },
    { validators: passwordMatchValidator }
  );

  get username()        { return this.form.controls.username; }
  get email()           { return this.form.controls.email; }
  get password()        { return this.form.controls.password; }
  get confirmPassword() { return this.form.controls.confirmPassword; }

  togglePassword(): void  { this.showPassword.update((v) => !v); }
  toggleConfirm(): void   { this.showConfirm.update((v) => !v); }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.loading.set(true);

    const credentials = { email: raw.email!, password: raw.password! };

    this.authApi.register({
        ...credentials,
        username: raw.username!,
      })
      .pipe(switchMap(() => this.authApi.login(credentials)))
      .subscribe({
        next: (response) => {
          this.authService.setSession(response);
          this.toast.success('Account created and signed in.');
          this.router.navigate(['/']);
        },
        error: (err: HttpErrorResponse) => {
          this.loading.set(false);
          const apiError = err.error as ApiError;
          this.toast.error(apiError?.message ?? 'Registration failed. Please try again.');
        },
        complete: () => this.loading.set(false),
      });
  }
}
