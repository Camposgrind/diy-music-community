import { ComponentRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MemberModalComponent, MemberModalForm } from './member-modal.component';

describe('MemberModalComponent', () => {
  let fixture: ComponentFixture<MemberModalComponent>;
  let componentRef: ComponentRef<MemberModalComponent>;
  let component: MemberModalComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [MemberModalComponent] }).compileComponents();
    fixture = TestBed.createComponent(MemberModalComponent);
    componentRef = fixture.componentRef;
    component = fixture.componentInstance;
  });

  it('should open empty for a current member and prevent invalid saving', () => {
    componentRef.setInput('mode', 'create');
    componentRef.setInput('memberType', 'current');
    fixture.detectChanges();
    expect(component.form.invalid).toBe(true);
    expect((fixture.nativeElement.querySelector('[data-testid="save-member"]') as HTMLButtonElement).disabled).toBe(true);
  });

  it('should preload an existing past member', () => {
    const initial: MemberModalForm = { name: 'Mick Harris', instrument: 'Drums', startYear: 1985, endYear: 1987, memberType: 'past' };
    componentRef.setInput('mode', 'edit');
    componentRef.setInput('memberType', 'past');
    componentRef.setInput('initialData', initial);
    fixture.detectChanges();
    expect(component.form.getRawValue()).toMatchObject({ ...initial, startYear: '1985', endYear: '1987' });
  });

  it('should emit a valid past member', () => {
    let emitted: unknown;
    component.save.subscribe((value) => (emitted = value));
    componentRef.setInput('mode', 'create');
    componentRef.setInput('memberType', 'past');
    fixture.detectChanges();
    component.form.setValue({ name: 'Mick Harris', instrument: 'Drums', startYear: '1985', endYear: '1987', memberType: 'past' });
    component.onSave();
    expect(emitted).toEqual({ name: 'Mick Harris', instrument: 'Drums', startYear: 1985, endYear: 1987, memberType: 'past' });
  });
});
