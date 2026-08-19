import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { BandMembersComponent } from './band-members.component';
import { BandMemberModel } from '../../../infrastructure/api/models';

const mockMembers: BandMemberModel[] = [
  { id: 'm1', name: 'Barney Greenway', instrument: 'Vocals', startYear: 1989, endYear: null, isCurrent: true, isLastKnownLineup: false, otherBands: [] },
  { id: 'm2', name: 'Shane Embury', instrument: 'Bass', startYear: 1987, endYear: null, isCurrent: true, isLastKnownLineup: false, otherBands: [] },
];

describe('BandMembersComponent', () => {
  let fixture: ComponentFixture<BandMembersComponent>;
  let componentRef: ComponentRef<BandMembersComponent>;
  let el: HTMLElement;

  beforeEach(async () => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BandMembersComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BandMembersComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('members', mockMembers);
    componentRef.setInput('title', 'Current Members');
    componentRef.setInput('memberType', 'current');
    fixture.detectChanges();
    el = fixture.nativeElement;
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the provided title', () => {
    expect(el.textContent).toContain('Current Members');
  });

  it('should render one row per member', () => {
    expect(el.textContent).toContain('Barney Greenway');
    expect(el.textContent).toContain('Shane Embury');
  });

  it('should render member instrument', () => {
    expect(el.textContent).toContain('Vocals');
    expect(el.textContent).toContain('Bass');
  });

  it('should render empty list when no members are provided', () => {
    componentRef.setInput('members', []);
    fixture.detectChanges();
    expect(el.textContent).not.toContain('Barney Greenway');
  });

  it('should show management controls only for an Admin', () => {
    expect(el.querySelector('[data-testid="add-member"]')).toBeNull();
    componentRef.setInput('isAdmin', true);
    fixture.detectChanges();
    expect(el.querySelector('[data-testid="add-member"]')).toBeTruthy();
    expect(el.querySelector('[data-testid="edit-member-m1"]')).toBeTruthy();
    expect(el.querySelector('[data-testid="delete-member-m1"]')).toBeTruthy();
  });

  it('should emit the selected member when an Admin requests deletion', () => {
    componentRef.setInput('isAdmin', true);
    const deleteMember = vi.fn();
    fixture.componentInstance.deleteMember.subscribe(deleteMember);
    fixture.detectChanges();

    (el.querySelector('[data-testid="delete-member-m1"]') as HTMLButtonElement).click();

    expect(deleteMember).toHaveBeenCalledWith(mockMembers[0]);
  });
});
