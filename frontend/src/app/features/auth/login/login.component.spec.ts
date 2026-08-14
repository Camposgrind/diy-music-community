import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { LoginComponent } from './login.component';
import { AuthApiService } from '../../../infrastructure/api/auth-api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Router } from '@angular/router';

describe('LoginComponent', () => {
  let fixture: ComponentFixture<LoginComponent>;
  let component: LoginComponent;
  let loginSpy: ReturnType<typeof vi.fn>;
  let setSessionSpy: ReturnType<typeof vi.fn>;
  let toastErrorSpy: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    loginSpy      = vi.fn();
    setSessionSpy = vi.fn();
    toastErrorSpy = vi.fn();

    TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideRouter([]),
        { provide: AuthApiService,  useValue: { login: loginSpy } },
        { provide: AuthService,     useValue: { setSession: setSessionSpy } },
        { provide: ToastService,    useValue: { error: toastErrorSpy } },
      ],
    });

    fixture   = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should start with an invalid form', () => {
    expect(component.form.invalid).toBe(true);
  });

  it('should be valid when both fields are filled in', () => {
    component.emailOrUsername.setValue('user@test.com');
    component.password.setValue('Password1!');
    expect(component.form.valid).toBe(true);
  });

  it('should mark fields as touched when submitting an invalid form', () => {
    component.onSubmit();
    expect(component.emailOrUsername.touched).toBe(true);
    expect(component.password.touched).toBe(true);
  });

  it('should NOT call the API when the form is invalid', () => {
    component.onSubmit();
    expect(loginSpy).not.toHaveBeenCalled();
  });

  it('should send email field when the input contains @', () => {
    loginSpy.mockReturnValue(of({ token: 't', expiresAt: '' }));
    component.emailOrUsername.setValue('a@a.com');
    component.password.setValue('Password1!');
    component.onSubmit();
    expect(loginSpy).toHaveBeenCalledWith(
      expect.objectContaining({ email: 'a@a.com', username: undefined })
    );
  });

  it('should send username field when the input has no @', () => {
    loginSpy.mockReturnValue(of({ token: 't', expiresAt: '' }));
    component.emailOrUsername.setValue('camposgrind');
    component.password.setValue('Password1!');
    component.onSubmit();
    expect(loginSpy).toHaveBeenCalledWith(
      expect.objectContaining({ username: 'camposgrind', email: undefined })
    );
  });

  it('should call setSession and navigate to / on successful login', () => {
    const res = { token: 'jwt', expiresAt: '' };
    loginSpy.mockReturnValue(of(res));
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    component.emailOrUsername.setValue('a@a.com');
    component.password.setValue('Password1!');
    component.onSubmit();
    expect(setSessionSpy).toHaveBeenCalledWith(res);
    expect(navSpy).toHaveBeenCalledWith(['/']);
  });

  it('should show toast error when login fails with API message', () => {
    const err = new HttpErrorResponse({ error: { message: 'Invalid credentials' }, status: 401 });
    loginSpy.mockReturnValue(throwError(() => err));
    component.emailOrUsername.setValue('a@a.com');
    component.password.setValue('Password1!');
    component.onSubmit();
    expect(toastErrorSpy).toHaveBeenCalledWith('Invalid credentials');
  });

  it('should show fallback toast error when no API message is present', () => {
    const err = new HttpErrorResponse({ error: {}, status: 500 });
    loginSpy.mockReturnValue(throwError(() => err));
    component.emailOrUsername.setValue('a@a.com');
    component.password.setValue('Password1!');
    component.onSubmit();
    expect(toastErrorSpy).toHaveBeenCalledWith('Login failed. Please try again.');
  });

  it('should toggle showPassword signal', () => {
    expect(component.showPassword()).toBe(false);
    component.togglePasswordVisibility();
    expect(component.showPassword()).toBe(true);
    component.togglePasswordVisibility();
    expect(component.showPassword()).toBe(false);
  });
});
