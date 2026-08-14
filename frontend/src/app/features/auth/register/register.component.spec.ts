import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { RegisterComponent } from './register.component';
import { AuthApiService } from '../../../infrastructure/api/auth-api.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Router } from '@angular/router';

import { Component } from '@angular/core';

@Component({ template: '', standalone: true })
class DummyComponent {}

describe('RegisterComponent', () => {
  let fixture: ComponentFixture<RegisterComponent>;
  let component: RegisterComponent;
  let registerSpy: ReturnType<typeof vi.fn>;
  let successSpy: ReturnType<typeof vi.fn>;
  let errorSpy: ReturnType<typeof vi.fn>;

  const fillValid = () => {
    component.username.setValue('CamposGrind');
    component.email.setValue('campos@test.com');
    component.password.setValue('Password1!');
    component.confirmPassword.setValue('Password1!');
  };

  beforeEach(() => {
    registerSpy = vi.fn();
    successSpy  = vi.fn();
    errorSpy    = vi.fn();

    TestBed.configureTestingModule({
      imports: [RegisterComponent],
      providers: [
        provideRouter([{ path: 'login', component: DummyComponent }]),
        { provide: AuthApiService, useValue: { register: registerSpy } },
        { provide: ToastService,   useValue: { success: successSpy, error: errorSpy } },
      ],
    });

    fixture   = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should start with an invalid form', () => {
    expect(component.form.invalid).toBe(true);
  });

  it('username should be invalid when shorter than 3 characters', () => {
    component.username.setValue('ab');
    expect(component.username.invalid).toBe(true);
  });

  it('username should reject invalid characters', () => {
    component.username.setValue('bad name!');
    expect(component.username.invalid).toBe(true);
  });

  it('username should accept letters, numbers, underscores and hyphens', () => {
    component.username.setValue('good_User-123');
    expect(component.username.valid).toBe(true);
  });

  it('email should be invalid for a bad format', () => {
    component.email.setValue('notanemail');
    expect(component.email.invalid).toBe(true);
  });

  it('password should be invalid when it lacks uppercase', () => {
    component.password.setValue('password1!');
    expect(component.password.invalid).toBe(true);
  });

  it('password should be invalid when it lacks a number', () => {
    component.password.setValue('Password!');
    expect(component.password.invalid).toBe(true);
  });

  it('password should be invalid when it lacks a special character', () => {
    component.password.setValue('Password1');
    expect(component.password.invalid).toBe(true);
  });

  it('form should be invalid when passwords do not match', () => {
    component.username.setValue('User1');
    component.email.setValue('a@a.com');
    component.password.setValue('Password1!');
    component.confirmPassword.setValue('Different1!');
    expect(component.form.invalid).toBe(true);
    expect(component.form.errors?.['passwordMismatch']).toBe(true);
  });

  it('form should be valid when all fields are correct', () => {
    fillValid();
    expect(component.form.valid).toBe(true);
  });

  it('should mark fields as touched when submitting an invalid form', () => {
    component.onSubmit();
    expect(component.username.touched).toBe(true);
  });

  it('should NOT call the API when the form is invalid', () => {
    component.onSubmit();
    expect(registerSpy).not.toHaveBeenCalled();
  });

  it('should show success toast and navigate to /login on success', () => {
    registerSpy.mockReturnValue(of(undefined));
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fillValid();
    component.onSubmit();
    expect(successSpy).toHaveBeenCalledWith('Account created! You can now sign in.');
    expect(navSpy).toHaveBeenCalledWith(['/login']);
  });

  it('should call register with the correct payload', () => {
    registerSpy.mockReturnValue(of(undefined));
    fillValid();
    component.onSubmit();
    expect(registerSpy).toHaveBeenCalledWith({
      email: 'campos@test.com',
      username: 'CamposGrind',
      password: 'Password1!',
    });
  });

  it('should show toast error with API message on failure', () => {
    const err = new HttpErrorResponse({ error: { message: 'Email already registered' }, status: 409 });
    registerSpy.mockReturnValue(throwError(() => err));
    fillValid();
    component.onSubmit();
    expect(errorSpy).toHaveBeenCalledWith('Email already registered');
  });

  it('should show fallback error when no API message is present', () => {
    const err = new HttpErrorResponse({ error: {}, status: 500 });
    registerSpy.mockReturnValue(throwError(() => err));
    fillValid();
    component.onSubmit();
    expect(errorSpy).toHaveBeenCalledWith('Registration failed. Please try again.');
  });

  it('should toggle showPassword', () => {
    expect(component.showPassword()).toBe(false);
    component.togglePassword();
    expect(component.showPassword()).toBe(true);
  });

  it('should toggle showConfirm independently', () => {
    expect(component.showConfirm()).toBe(false);
    component.toggleConfirm();
    expect(component.showConfirm()).toBe(true);
    expect(component.showPassword()).toBe(false);
  });
});
