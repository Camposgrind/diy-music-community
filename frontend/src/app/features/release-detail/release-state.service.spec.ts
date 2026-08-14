import { TestBed } from '@angular/core/testing';
import { ReleaseStateService } from './release-state.service';

describe('ReleaseStateService', () => {
  let service: ReleaseStateService;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReleaseStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('bandId() is null initially', () => {
    expect(service.bandId()).toBeNull();
  });

  it('saveBandId() stores the provided id', () => {
    service.saveBandId('band-42');
    expect(service.bandId()).toBe('band-42');
  });

  it('saveBandId() overwrites a previously saved id', () => {
    service.saveBandId('band-1');
    service.saveBandId('band-99');
    expect(service.bandId()).toBe('band-99');
  });

  it('clear() resets bandId to null', () => {
    service.saveBandId('band-42');
    service.clear();
    expect(service.bandId()).toBeNull();
  });
});
