import { Injectable, signal, computed, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { AuthUser, LoginResponse } from '../../infrastructure/api/models';

const TOKEN_KEY = 'dmc_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly router = inject(Router);

  private readonly _token = signal<string | null>(this.loadToken());
  private readonly _user = signal<AuthUser | null>(this.decodeToken(this.loadToken()));

  readonly isAuthenticated = computed(() => {
    const user = this._user();
    if (!user) return false;
    return user.expiresAt > new Date();
  });

  readonly currentUser = computed(() => this._user());

  readonly isAdmin = computed(() =>
    this._user()?.roles.includes('Admin') ?? false
  );

  readonly isModerator = computed(() =>
    this._user()?.roles.includes('Moderator') ?? false
  );

  setSession(response: LoginResponse): void {
    if (isPlatformBrowser(this.platformId)) {
      sessionStorage.setItem(TOKEN_KEY, response.token);
    }
    this._token.set(response.token);
    this._user.set(this.decodeToken(response.token));
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      sessionStorage.removeItem(TOKEN_KEY);
    }
    this._token.set(null);
    this._user.set(null);
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return this._token();
  }

  private loadToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return sessionStorage.getItem(TOKEN_KEY);
    }
    return null;
  }

  private decodeToken(token: string | null): AuthUser | null {
    if (!token) return null;

    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;

      const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));

      const roles: string[] = Array.isArray(payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
        ? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        : payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
          ? [payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']]
          : Array.isArray(payload['role'])
            ? payload['role']
            : payload['role']
              ? [payload['role']]
              : [];

      const expiresAt = new Date((payload['exp'] as number) * 1000);

      return {
        sub: payload['sub'] as string,
        username: (payload['unique_name'] ?? payload['name'] ?? payload['email']) as string,
        email: payload['email'] as string,
        roles,
        expiresAt,
      };
    } catch {
      return null;
    }
  }
}
