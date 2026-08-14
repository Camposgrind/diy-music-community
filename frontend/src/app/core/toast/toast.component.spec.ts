import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { vi } from 'vitest';
import { ToastComponent } from './toast.component';
import { ToastService, Toast } from './toast.service';

describe('ToastComponent', () => {
  let fixture: ComponentFixture<ToastComponent>;

  const setup = (initialToasts: Toast[] = []) => {
    const toastsSig = signal<Toast[]>(initialToasts);
    const dismissSpy = vi.fn();
    const fakeService = { toasts: toastsSig, dismiss: dismissSpy };

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [ToastComponent],
      providers: [{ provide: ToastService, useValue: fakeService }],
    });

    fixture = TestBed.createComponent(ToastComponent);
    fixture.detectChanges();
    return { toastsSig, dismissSpy };
  };

  it('should render nothing when there are no toasts', () => {
    setup([]);
    const els = fixture.nativeElement.querySelectorAll('.toast');
    expect(els.length).toBe(0);
  });

  it('should render one element per toast', () => {
    setup([
      { id: 1, message: 'A', type: 'info' },
      { id: 2, message: 'B', type: 'error' },
      { id: 3, message: 'C', type: 'success' },
    ]);
    expect(fixture.nativeElement.querySelectorAll('.toast').length).toBe(3);
  });

  it('should apply the correct type modifier class', () => {
    setup([{ id: 1, message: 'Oops', type: 'error' }]);
    const el: HTMLElement = fixture.nativeElement.querySelector('.toast');
    expect(el.classList.contains('toast--error')).toBe(true);
  });

  it('should display the toast message', () => {
    setup([{ id: 1, message: 'Account created!', type: 'success' }]);
    const text: string = fixture.nativeElement.querySelector('.toast__message').textContent;
    expect(text.trim()).toBe('Account created!');
  });

  it('should call dismiss when the toast is clicked', () => {
    const { dismissSpy } = setup([{ id: 42, message: 'Click me', type: 'info' }]);
    const el: HTMLElement = fixture.nativeElement.querySelector('.toast');
    el.click();
    expect(dismissSpy).toHaveBeenCalledWith(42);
  });

  it('should render the label text for each type', () => {
    const cases: Array<[Toast['type'], string]> = [['error', 'Error'], ['success', 'Success'], ['info', 'Info']];
    for (const [type, expected] of cases) {
      setup([{ id: 1, message: 'msg', type }]);
      const label: string = fixture.nativeElement.querySelector('.toast__label').textContent;
      expect(label.trim()).toBe(expected);
    }
  });
});
