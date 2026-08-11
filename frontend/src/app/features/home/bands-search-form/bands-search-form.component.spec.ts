import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { BandsSearchFormComponent, BandSearchFilters } from './bands-search-form.component';
import { GenreModel } from '../../../infrastructure/api/models';

describe('BandsSearchFormComponent', () => {
  let fixture: ComponentFixture<BandsSearchFormComponent>;
  let componentRef: ComponentRef<BandsSearchFormComponent>;
  let component: BandsSearchFormComponent;
  let el: HTMLElement;

  const mockCountries = ['Spain', 'Japan', 'Brazil'];
  const mockGenres: GenreModel[] = [
    { id: 'g1', name: 'Grindcore' },
    { id: 'g2', name: 'Crust' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BandsSearchFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BandsSearchFormComponent);
    componentRef = fixture.componentRef;
    component = fixture.componentInstance;
    el = fixture.nativeElement;

    componentRef.setInput('countries', mockCountries);
    componentRef.setInput('genres', mockGenres);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display country options from input', () => {
    const countrySelect = el.querySelector('#country') as HTMLSelectElement;
    const options = countrySelect.querySelectorAll('option');
    // First option is "All Countries" placeholder
    expect(options.length).toBe(4);
    expect(options[1].textContent).toContain('Spain');
    expect(options[2].textContent).toContain('Japan');
  });

  it('should display genre options from input', () => {
    const genreSelect = el.querySelector('#genre') as HTMLSelectElement;
    const options = genreSelect.querySelectorAll('option');
    expect(options.length).toBe(3);
    expect(options[1].textContent).toContain('Grindcore');
    expect(options[2].textContent).toContain('Crust');
  });

  it('should emit search event with form values on submit', () => {
    let emitted: BandSearchFilters | undefined;
    component.search.subscribe((f: BandSearchFilters) => (emitted = f));

    component.form.patchValue({ name: 'Discharge', country: 'Spain', genreId: 'g1' });
    component.onSearch();

    expect(emitted).toEqual({ name: 'Discharge', country: 'Spain', genreId: 'g1' });
  });

  it('should emit empty strings for unfilled fields', () => {
    let emitted: BandSearchFilters | undefined;
    component.search.subscribe((f: BandSearchFilters) => (emitted = f));

    component.onSearch();

    expect(emitted).toEqual({ name: '', country: '', genreId: '' });
  });

  it('should emit reset event and clear form on reset', () => {
    let resetCalled = false;
    component.reset.subscribe(() => (resetCalled = true));

    component.form.patchValue({ name: 'test', country: 'Japan', genreId: 'g2' });
    component.onReset();

    expect(resetCalled).toBe(true);
    expect(component.form.value).toEqual({ name: '', country: '', genreId: '' });
  });

  it('should have a band name input field', () => {
    const input = el.querySelector('#bandName') as HTMLInputElement;
    expect(input).toBeTruthy();
    expect(input.type).toBe('text');
  });

  it('should show genres error message when genresError is set', () => {
    componentRef.setInput('genresError', 'Could not load genres — is the backend running?');
    fixture.detectChanges();
    const errorEl = el.querySelector('.search-form__field-error');
    expect(errorEl).toBeTruthy();
    expect(errorEl?.textContent).toContain('Could not load genres');
  });

  it('should not inject any services (presentational component)', () => {
    // Verify no HttpClient or API service injection by checking the component has no ngOnInit HTTP calls
    // The component should only have inputs and outputs
    expect((component as any).http).toBeUndefined();
    expect((component as any).genresApi).toBeUndefined();
  });
});
