import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { HeaderComponent } from './header.component';
import { AuthService } from '../../../core/auth/auth.service';
import { SearchStateService } from '../../../features/home/search-state.service';
import { Router } from '@angular/router';
import { AuthUser } from '../../../infrastructure/api/models';

const mockUser: AuthUser = {
  sub: 'u1',
  username: 'camposgrind',
  email: 'camposgrind@gmail.com',
  roles: [],
  expiresAt: new Date(Date.now() + 3_600_000),
};

describe('HeaderComponent', () => {
  let fixture: ComponentFixture<HeaderComponent>;

  const setup = (authenticated: boolean, user: AuthUser | null = null) => {
    const fakeAuth = {
      isAuthenticated: vi.fn().mockReturnValue(authenticated),
      currentUser: vi.fn().mockReturnValue(user),
      logout: vi.fn(),
    };
    const fakeSearch = { clear: vi.fn() };

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [HeaderComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: fakeAuth },
        { provide: SearchStateService, useValue: fakeSearch },
      ],
    });

    fixture = TestBed.createComponent(HeaderComponent);
    fixture.detectChanges();
    return { fakeAuth, fakeSearch };
  };

  describe('when not authenticated', () => {
    it('should show the Login link', () => {
      setup(false);
      const links: HTMLAnchorElement[] = Array.from(fixture.nativeElement.querySelectorAll('a'));
      const loginLink = links.find(l => l.textContent?.trim().toLowerCase() === 'login');
      expect(loginLink).toBeTruthy();
    });

    it('should NOT show the logout button', () => {
      setup(false);
      expect(fixture.nativeElement.querySelector('.header__logout')).toBeNull();
    });

    it('should NOT show the username element', () => {
      setup(false);
      expect(fixture.nativeElement.querySelector('.header__user')).toBeNull();
    });
  });

  describe('when authenticated', () => {
    it('should show the username', () => {
      setup(true, mockUser);
      const userEl: HTMLElement = fixture.nativeElement.querySelector('.header__user');
      expect(userEl.textContent).toContain('camposgrind');
    });

    it('should NOT show the Login link', () => {
      setup(true, mockUser);
      const links: HTMLAnchorElement[] = Array.from(fixture.nativeElement.querySelectorAll('a'));
      const loginLink = links.find(l => l.textContent?.trim().toLowerCase() === 'login');
      expect(loginLink).toBeFalsy();
    });

    it('should show the logout button', () => {
      setup(true, mockUser);
      expect(fixture.nativeElement.querySelector('.header__logout')).toBeTruthy();
    });
  });

  it('clearAndGoHome() should clear search state and navigate to /', () => {
    const { fakeSearch } = setup(false);
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.componentInstance.clearAndGoHome();
    expect(fakeSearch.clear).toHaveBeenCalled();
    expect(navSpy).toHaveBeenCalledWith(['/']);
  });

  it('logout() should call auth.logout()', () => {
    const { fakeAuth } = setup(true, mockUser);
    fixture.componentInstance.logout();
    expect(fakeAuth.logout).toHaveBeenCalled();
  });
});
