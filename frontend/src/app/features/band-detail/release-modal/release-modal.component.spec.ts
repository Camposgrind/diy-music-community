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
    const initial: ReleaseModalForm = { title: 'Scum', releaseType: 'Album', year: 1987 };
    componentRef.setInput('mode', 'edit');
    componentRef.setInput('initialData', initial);
    fixture.detectChanges();
    expect(component.form.getRawValue()).toEqual({ title: 'Scum', releaseType: 'Album', year: '1987' });
  });

  it('should emit valid release fields', () => {
    let emitted: unknown;
    component.save.subscribe((value) => (emitted = value));
    componentRef.setInput('mode', 'create');
    fixture.detectChanges();
    component.form.setValue({ title: 'Scum', releaseType: 'Album', year: '1987' });
    component.onSave();
    expect(emitted).toEqual({ title: 'Scum', releaseType: 'Album', year: 1987 });
  });
});
