export interface LoginRequest {
  email?: string;
  username?: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}

/** Claims decoded from the JWT payload */
export interface AuthUser {
  sub: string;
  username: string;
  email: string;
  roles: string[];
  expiresAt: Date;
}
