import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { PLATFORM_ID } from '@angular/core';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { AuthService } from './auth.service';

function buildJwt(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
    .replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_');
  const body = btoa(JSON.stringify(payload))
    .replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_');
  return `${header}.${body}.fakesig`;
}

const futureExp = Math.floor(Date.now() / 1000) + 3600;
const pastExp = Math.floor(Date.now() / 1000) - 3600;

const validToken = buildJwt({
  sub: 'user-123',
  unique_name: 'camposgrind',
  email: 'camposgrind@gmail.com',
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': 'User',
  exp: futureExp,
});

const adminToken = buildJwt({
  sub: 'admin-456',
  unique_name: 'adminuser',
  email: 'admin@example.com',
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': ['Admin', 'Moderator'],
  exp: futureExp,
});

const expiredToken = buildJwt({
  sub: 'user-789',
  unique_name: 'expired',
  email: 'expired@example.com',
  role: 'User',
  exp: pastExp,
});

describe('AuthService', () => {
  let service: AuthService;
  let navigateSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideRouter([]),
        { provide: PLATFORM_ID, useValue: 'browser' },
      ],
    });

    const router = TestBed.inject(Router);
    navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    service = TestBed.inject(AuthService);
  });

  afterEach(() => sessionStorage.clear());

  it('starts unauthenticated', () => {
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });

  it('authenticates after setSession', () => {
    service.setSession({ token: validToken, expiresAt: '' });
    expect(service.isAuthenticated()).toBe(true);
    expect(service.currentUser()?.username).toBe('camposgrind');
    expect(service.currentUser()?.email).toBe('camposgrind@gmail.com');
  });

  it('persists token to sessionStorage', () => {
    service.setSession({ token: validToken, expiresAt: '' });
    expect(sessionStorage.getItem('dmc_token')).toBe(validToken);
  });

  it('isAdmin false for regular user', () => {
    service.setSession({ token: validToken, expiresAt: '' });
    expect(service.isAdmin()).toBe(false);
  });

  it('isAdmin true for Admin role', () => {
    service.setSession({ token: adminToken, expiresAt: '' });
    expect(service.isAdmin()).toBe(true);
  });

  it('isModerator true for Moderator role', () => {
    service.setSession({ token: adminToken, expiresAt: '' });
    expect(service.isModerator()).toBe(true);
  });

  it('isAuthenticated false for expired token', () => {
    service.setSession({ token: expiredToken, expiresAt: '' });
    expect(service.isAuthenticated()).toBe(false);
  });

  it('logout clears state and navigates home', () => {
    service.setSession({ token: validToken, expiresAt: '' });
    service.logout();
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(sessionStorage.getItem('dmc_token')).toBeNull();
    expect(navigateSpy).toHaveBeenCalledWith(['/']);
  });

  it('getToken null when not authenticated', () => {
    expect(service.getToken()).toBeNull();
  });

  it('getToken returns token after setSession', () => {
    service.setSession({ token: validToken, expiresAt: '' });
    expect(service.getToken()).toBe(validToken);
  });

  it('handles malformed token gracefully', () => {
    service.setSession({ token: 'bad.token', expiresAt: '' });
    expect(service.currentUser()).toBeNull();
  });

  it('reads roles from plain role claim', () => {
    const t = buildJwt({ sub: 'u1', unique_name: 'u', email: 'u@u.com', role: 'Admin', exp: futureExp });
    service.setSession({ token: t, expiresAt: '' });
    expect(service.isAdmin()).toBe(true);
  });

  it('falls back to email when unique_name absent', () => {
    const t = buildJwt({ sub: 'u2', email: 'fb@x.com', exp: futureExp });
    service.setSession({ token: t, expiresAt: '' });
    expect(service.currentUser()?.username).toBe('fb@x.com');
  });
});
