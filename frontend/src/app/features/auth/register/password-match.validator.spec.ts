import { FormControl, FormGroup } from '@angular/forms';
import { passwordMatchValidator } from './password-match.validator';

describe('passwordMatchValidator', () => {
  const buildGroup = (password: string, confirmPassword: string) =>
    new FormGroup(
      {
        password: new FormControl(password),
        confirmPassword: new FormControl(confirmPassword),
      },
      { validators: passwordMatchValidator }
    );

  it('should return null when passwords match', () => {
    const group = buildGroup('Password1!', 'Password1!');
    expect(group.errors).toBeNull();
  });

  it('should return { passwordMismatch: true } when passwords differ', () => {
    const group = buildGroup('Password1!', 'Different1!');
    expect(group.errors).toEqual({ passwordMismatch: true });
  });

  it('should return { passwordMismatch: true } when confirmPassword is empty', () => {
    const group = buildGroup('Password1!', '');
    expect(group.errors).toEqual({ passwordMismatch: true });
  });

  it('should return null when both passwords are empty strings', () => {
    const group = buildGroup('', '');
    expect(group.errors).toBeNull();
  });

  it('should be case-sensitive', () => {
    const group = buildGroup('password1!', 'Password1!');
    expect(group.errors).toEqual({ passwordMismatch: true });
  });
});
