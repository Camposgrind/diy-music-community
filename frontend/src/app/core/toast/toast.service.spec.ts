import { TestBed } from '@angular/core/testing';
import { ToastService } from './toast.service';

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ToastService);
  });

  it('should start with an empty toast list', () => {
    expect(service.toasts()).toEqual([]);
  });

  it('show() should add a toast with the correct type and message', () => {
    service.show('Hello', 'info');
    expect(service.toasts().length).toBe(1);
    expect(service.toasts()[0].message).toBe('Hello');
    expect(service.toasts()[0].type).toBe('info');
  });

  it('error() should add an error toast', () => {
    service.error('Something went wrong');
    expect(service.toasts()[0].type).toBe('error');
    expect(service.toasts()[0].message).toBe('Something went wrong');
  });

  it('success() should add a success toast', () => {
    service.success('Done!');
    expect(service.toasts()[0].type).toBe('success');
  });

  it('dismiss() should remove a toast by id', () => {
    service.show('One', 'info');
    service.show('Two', 'info');
    const id = service.toasts()[0].id;
    service.dismiss(id);
    expect(service.toasts().length).toBe(1);
    expect(service.toasts()[0].message).toBe('Two');
  });

  it('should auto-dismiss after the specified duration', () => {
    vi.useFakeTimers();
    service.show('Auto dismiss', 'info', 2000);
    expect(service.toasts().length).toBe(1);
    vi.advanceTimersByTime(2000);
    expect(service.toasts().length).toBe(0);
    vi.useRealTimers();
  });

  it('should auto-dismiss after default 4000 ms', () => {
    vi.useFakeTimers();
    service.show('Default duration', 'error');
    vi.advanceTimersByTime(3999);
    expect(service.toasts().length).toBe(1);
    vi.advanceTimersByTime(1);
    expect(service.toasts().length).toBe(0);
    vi.useRealTimers();
  });

  it('should assign unique incremental ids to toasts', () => {
    service.show('A', 'info');
    service.show('B', 'info');
    const ids = service.toasts().map(t => t.id);
    expect(ids[0]).toBeLessThan(ids[1]);
    expect(new Set(ids).size).toBe(2);
  });

  it('should support multiple concurrent toasts', () => {
    service.show('First', 'info');
    service.show('Second', 'error');
    service.show('Third', 'success');
    expect(service.toasts().length).toBe(3);
  });
});
