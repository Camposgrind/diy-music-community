import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthApiService } from './auth-api.service';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, LoginResponse } from './models';

describe('AuthApiService', () => {
  let service: AuthApiService;
  let httpMock: HttpTestingController;
  const base = `${environment.apiBaseUrl}/auth`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  describe('login()', () => {
    it('should POST to /auth/login and return LoginResponse', () => {
      const req: LoginRequest = { email: 'a@a.com', password: 'Pass123!' };
      const mockRes: LoginResponse = { token: 'jwt', expiresAt: '' };

      service.login(req).subscribe(res => expect(res).toEqual(mockRes));

      const httpReq = httpMock.expectOne(`${base}/login`);
      expect(httpReq.request.method).toBe('POST');
      expect(httpReq.request.body).toEqual(req);
      httpReq.flush(mockRes);
    });

    it('should POST with username when email is omitted', () => {
      const req: LoginRequest = { username: 'camposgrind', password: 'Pass123!' };
      service.login(req).subscribe();

      const httpReq = httpMock.expectOne(`${base}/login`);
      expect(httpReq.request.body).toEqual(req);
      httpReq.flush({ token: 't', expiresAt: '' });
    });
  });

  describe('register()', () => {
    it('should POST to /auth/register', () => {
      const req: RegisterRequest = { email: 'new@user.com', username: 'newuser', password: 'Pass123!' };
      service.register(req).subscribe();

      const httpReq = httpMock.expectOne(`${base}/register`);
      expect(httpReq.request.method).toBe('POST');
      expect(httpReq.request.body).toEqual(req);
      httpReq.flush(null, { status: 201, statusText: 'Created' });
    });

    it('should propagate HTTP error responses', () => {
      const req: RegisterRequest = { email: 'dup@dup.com', username: 'dup', password: 'Pass123!' };
      let errorCaught = false;

      service.register(req).subscribe({ error: () => (errorCaught = true) });

      httpMock.expectOne(`${base}/register`).flush(
        { message: 'Email already registered' },
        { status: 409, statusText: 'Conflict' }
      );
      expect(errorCaught).toBe(true);
    });
  });
});
