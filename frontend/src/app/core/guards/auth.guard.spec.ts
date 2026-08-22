import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { authGuard } from './auth.guard';
import { AuthService } from '../auth/auth.service';

describe('authGuard', () => {
  const runGuard = () =>
    TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));

  const setup = (authenticated: boolean) => {
    const isAuthSpy = vi.fn().mockReturnValue(authenticated);
    const createUrlTreeSpy = vi.fn().mockReturnValue({ toString: () => '/admin/login' } as unknown as UrlTree);

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: { isAuthenticated: isAuthSpy } },
      ],
    });

    const router = TestBed.inject(Router);
    vi.spyOn(router, 'createUrlTree').mockImplementation(createUrlTreeSpy);
    return { isAuthSpy, createUrlTreeSpy };
  };

  it('should allow navigation when the user is authenticated', () => {
    setup(true);
    expect(runGuard()).toBe(true);
  });

  it('should redirect to /admin/login when the user is not authenticated', () => {
    const { createUrlTreeSpy } = setup(false);
    const result = runGuard();
    expect(createUrlTreeSpy).toHaveBeenCalledWith(['/admin/login']);
    expect(result).toBeTruthy();
  });
});
