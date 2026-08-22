import { routes } from './app.routes';

describe('application routes', () => {
  it('should expose login only through the administrator entry route', () => {
    const paths = routes.map(route => route.path);

    expect(paths).toContain('admin/login');
    expect(paths).not.toContain('login');
    expect(paths).not.toContain('register');
  });
});
