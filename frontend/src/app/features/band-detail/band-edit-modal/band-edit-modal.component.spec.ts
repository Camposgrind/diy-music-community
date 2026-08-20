import { ComponentRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GenreModel } from '../../../infrastructure/api/models';
import { BandEditModalComponent, BandGeneralEditForm } from './band-edit-modal.component';

describe('BandEditModalComponent', () => {
  let fixture: ComponentFixture<BandEditModalComponent>;
  let componentRef: ComponentRef<BandEditModalComponent>;
  let component: BandEditModalComponent;

  const initialData: BandGeneralEditForm = {
    name: 'Napalm Death', country: 'United Kingdom', location: 'Birmingham', formationYear: 1981,
    splitUpYear: null, genreId: 'genre-1', status: 'Active', musicUrlPortal: 'https://example.com', bandContact: 'contact@example.com',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [BandEditModalComponent] }).compileComponents();
    fixture = TestBed.createComponent(BandEditModalComponent);
    componentRef = fixture.componentRef;
    component = fixture.componentInstance;
    componentRef.setInput('countries', ['Spain', 'United Kingdom']);
    componentRef.setInput('genres', [{ id: 'genre-1', name: 'Grindcore' } satisfies GenreModel]);
    componentRef.setInput('initialData', initialData);
    fixture.detectChanges();
  });

  it('should preload the current general band data', () => {
    expect(component.form.getRawValue()).toMatchObject({ ...initialData, formationYear: '1981', splitUpYear: '' });
  });

  it('should disable saving while a required field is invalid', () => {
    component.form.controls.name.setValue('');
    fixture.detectChanges();

    expect((fixture.nativeElement.querySelector('[data-testid="save-band-edit"]') as HTMLButtonElement).disabled).toBe(true);
  });

  it('should emit edited general data without media fields', () => {
    let emitted: unknown;
    component.save.subscribe((value) => (emitted = value));
    component.onSave();

    expect(emitted).toMatchObject({ name: 'Napalm Death', formationYear: 1981, genreId: 'genre-1' });
    expect(emitted).not.toHaveProperty('logoImageUrl');
    expect(emitted).not.toHaveProperty('bandImageUrl');
  });

  it('should require a split-up year when the status changes to SplitUp', () => {
    component.form.controls.status.setValue('SplitUp');
    component.onStatusChange();
    fixture.detectChanges();

    expect(component.form.controls.splitUpYear.invalid).toBe(true);
    expect((fixture.nativeElement.querySelector('[data-testid="save-band-edit"]') as HTMLButtonElement).disabled).toBe(true);
  });
});
