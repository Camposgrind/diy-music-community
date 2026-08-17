import { ComponentRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GenreModel } from '../../../infrastructure/api/models';
import { BandCreateModalComponent } from './band-create-modal.component';

describe('BandCreateModalComponent', () => {
  let fixture: ComponentFixture<BandCreateModalComponent>;
  let componentRef: ComponentRef<BandCreateModalComponent>;
  let component: BandCreateModalComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [BandCreateModalComponent] }).compileComponents();
    fixture = TestBed.createComponent(BandCreateModalComponent);
    componentRef = fixture.componentRef;
    component = fixture.componentInstance;
    componentRef.setInput('countries', ['Spain', 'United Kingdom']);
    componentRef.setInput('genres', [{ id: 'genre-1', name: 'D-Beat' } satisfies GenreModel]);
    fixture.detectChanges();
  });

  it('should disable saving while required fields are empty', () => {
    const saveButton = fixture.nativeElement.querySelector('[data-testid="save-band"]') as HTMLButtonElement;
    expect(component.form.invalid).toBe(true);
    expect(saveButton.disabled).toBe(true);
  });

  it('should emit the general band data when the form is valid', () => {
    let emitted: unknown;
    component.save.subscribe((request) => (emitted = request));
    component.form.patchValue({
      name: 'Discharge', country: 'United Kingdom', genreId: 'genre-1', status: 'Active',
    });

    component.onSave();

    expect(emitted).toMatchObject({
      name: 'Discharge', country: 'United Kingdom', genreId: 'genre-1', status: 'Active',
    });
  });

  it('should provide a direct four-digit year entry and exclude media fields', () => {
    const yearInput = fixture.nativeElement.querySelector('#new-band-formation-year') as HTMLInputElement;

    expect(yearInput.type).toBe('text');
    expect(yearInput.placeholder).toBe('e.g. 1999');
    expect(fixture.nativeElement.querySelector('#new-band-logo')).toBeNull();
    expect(fixture.nativeElement.querySelector('#new-band-photo')).toBeNull();
  });

  it('should emit close when its close button is used', () => {
    let wasClosed = false;
    component.close.subscribe(() => (wasClosed = true));

    (fixture.nativeElement.querySelector('[data-testid="close-band-modal"]') as HTMLButtonElement).click();

    expect(wasClosed).toBe(true);
  });
});
