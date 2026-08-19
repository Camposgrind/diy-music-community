import { ComponentRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReleaseModalComponent, ReleaseModalForm } from './release-modal.component';

describe('ReleaseModalComponent', () => {
  let fixture: ComponentFixture<ReleaseModalComponent>;
  let componentRef: ComponentRef<ReleaseModalComponent>;
  let component: ReleaseModalComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ReleaseModalComponent] }).compileComponents();
    fixture = TestBed.createComponent(ReleaseModalComponent);
    componentRef = fixture.componentRef;
    component = fixture.componentInstance;
  });

  it('should start empty in create mode and prevent an invalid save', () => {
    componentRef.setInput('mode', 'create');
    fixture.detectChanges();
    expect(component.form.invalid).toBe(true);
    expect((fixture.nativeElement.querySelector('[data-testid="save-release"]') as HTMLButtonElement).disabled).toBe(true);
  });

  it('should preload an existing release in edit mode', () => {
    const initial: ReleaseModalForm = { title: 'Scum', releaseType: 'Album', releaseDate: null, year: 1987, labelText: null, formats: ['CD'] };
    componentRef.setInput('mode', 'edit');
    componentRef.setInput('initialData', initial);
    fixture.detectChanges();
    expect(component.form.getRawValue()).toEqual({ title: 'Scum', releaseType: 'Album', releaseDate: '', year: '1987', labelText: '', formats: ['CD'] });
  });

  it('should normalize an API date value for the native date input and preserve it on save', () => {
    componentRef.setInput('mode', 'edit');
    componentRef.setInput('initialData', { title: 'Scum', releaseType: 'Album', releaseDate: '1987-07-01T00:00:00Z', year: 1987, labelText: null, formats: [] });
    fixture.detectChanges();

    expect(component.form.controls.releaseDate.value).toBe('01/07/1987');
    let emitted: ReleaseModalForm | undefined;
    component.save.subscribe(value => (emitted = value));
    component.onSave();
    expect(emitted?.releaseDate).toBe('1987-07-01');
  });

  it('should mask a manually entered date and emit it as ISO format', () => {
    componentRef.setInput('mode', 'create');
    fixture.detectChanges();
    component.form.controls.title.setValue('Scum');

    component.onReleaseDateInput({ target: { value: '31121987' } } as unknown as Event);

    expect(component.form.controls.releaseDate.value).toBe('31/12/1987');
    let emitted: ReleaseModalForm | undefined;
    component.save.subscribe(value => (emitted = value));
    component.onSave();
    expect(emitted?.releaseDate).toBe('1987-12-31');
  });

  it('should select formats through visible chip controls', () => {
    componentRef.setInput('mode', 'create');
    fixture.detectChanges();
    component.toggleFormat('CD');
    component.toggleFormat('Vinyl12');
    fixture.detectChanges();

    expect(component.form.controls.formats.value).toEqual(['CD', 'Vinyl12']);
    expect(component.formatsSummary()).toBe('CD, 12" Vinyl');
    expect(fixture.nativeElement.querySelector('[data-testid="formats-chip-selector"]')).toBeTruthy();
  });

  it('should normalize every API display label to its matching format chip value', () => {
    componentRef.setInput('mode', 'edit');
    componentRef.setInput('initialData', {
      title: 'Scum',
      releaseType: 'Album',
      releaseDate: null,
      year: 1987,
      labelText: null,
      formats: ['7" Vinyl', '10" Vinyl', '12" Vinyl', 'Vinyl Lathe Cut', 'Vinyl (Other)', 'CD', 'CD-R', 'DVD', 'Cassette', 'Digital'] as unknown as ReleaseModalForm['formats'],
    });
    fixture.detectChanges();

    expect(component.form.controls.formats.value).toEqual(['Vinyl7', 'Vinyl10', 'Vinyl12', 'VinylLatheCut', 'VinylOther', 'CD', 'CDR', 'DVD', 'Cassette', 'Digital']);
    expect(component.isFormatSelected('Vinyl12')).toBe(true);
  });

  it('should emit valid release fields', () => {
    let emitted: unknown;
    component.save.subscribe((value) => (emitted = value));
    componentRef.setInput('mode', 'create');
    fixture.detectChanges();
    component.form.setValue({ title: 'Scum', releaseType: 'Album', releaseDate: '01/07/1987', year: '1987', labelText: 'Earache', formats: [] });
    component.onSave();
    expect(emitted).toEqual({ title: 'Scum', releaseType: 'Album', releaseDate: '1987-07-01', year: 1987, labelText: 'Earache', formats: [] });
  });
});
