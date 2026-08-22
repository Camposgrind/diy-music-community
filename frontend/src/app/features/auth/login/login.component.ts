import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthApiService } from '../../../infrastructure/api/auth-api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/toast/toast.service';
import { ApiError } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly authApi = inject(AuthApiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly toast = inject(ToastService);

  readonly loading = signal(false);
  readonly showPassword = signal(false);

  readonly form = new FormGroup({
    emailOrUsername: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
  });

  get emailOrUsername() { return this.form.controls.emailOrUsername; }
  get password() { return this.form.controls.password; }

  togglePasswordVisibility(): void {
    this.showPassword.update((v) => !v);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const value = raw.emailOrUsername!.trim();
    const isEmail = value.includes('@');

    this.loading.set(true);

    this.authApi
      .login({
        email: isEmail ? value : undefined,
        username: !isEmail ? value : undefined,
        password: raw.password!,
      })
      .subscribe({
        next: (response) => {
          this.authService.setSession(response);
          const returnUrl = this.safeReturnUrl();
          if (returnUrl) {
            this.router.navigateByUrl(returnUrl);
          } else {
            this.router.navigate(['/']);
          }
        },
        error: (err: HttpErrorResponse) => {
          this.loading.set(false);
          const apiError = err.error as ApiError;
          this.toast.error(apiError?.message ?? 'Login failed. Please try again.');
        },
        complete: () => this.loading.set(false),
      });
  }

  private safeReturnUrl(): string | null {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    if (!returnUrl || !returnUrl.startsWith('/') || returnUrl.startsWith('//') || returnUrl.includes('\\')) {
      return null;
    }

    return returnUrl;
  }
}
