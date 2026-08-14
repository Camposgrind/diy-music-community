import { TestBed } from '@angular/core/testing';
import { SearchStateService } from './search-state.service';
import { GetBandsQuery } from '../../infrastructure/api/models';

const mockQuery: GetBandsQuery = { name: 'Napalm', country: null, genreId: null };

describe('SearchStateService', () => {
  let service: SearchStateService;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});
    service = TestBed.inject(SearchStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('state() is null initially', () => {
    expect(service.state()).toBeNull();
  });

  it('save() stores the provided state', () => {
    service.save({ query: mockQuery, page: 2 });
    expect(service.state()).toEqual({ query: mockQuery, page: 2 });
  });

  it('save() overwrites a previously saved state', () => {
    service.save({ query: mockQuery, page: 1 });
    service.save({ query: { name: 'Discharge', country: null, genreId: null }, page: 3 });
    expect(service.state()?.page).toBe(3);
    expect(service.state()?.query.name).toBe('Discharge');
  });

  it('clear() resets state to null', () => {
    service.save({ query: mockQuery, page: 1 });
    service.clear();
    expect(service.state()).toBeNull();
  });
});
