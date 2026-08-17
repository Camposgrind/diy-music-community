import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { Router } from '@angular/router';
import { BandDiscographyComponent } from './band-discography.component';
import { BandReleaseModel } from '../../../infrastructure/api/models';
import { ReleaseStateService } from '../../release-detail/release-state.service';

const mockReleases: BandReleaseModel[] = [
  { id: 'r1', title: 'Scum', releaseType: 'Album', year: 1987 },
  { id: 'r2', title: 'From Enslavement to Obliteration', releaseType: 'Album', year: 1988 },
  { id: 'r3', title: 'The Peel Sessions', releaseType: 'EP', year: 1989 },
];

describe('BandDiscographyComponent', () => {
  let fixture: ComponentFixture<BandDiscographyComponent>;
  let componentRef: ComponentRef<BandDiscographyComponent>;
  let component: BandDiscographyComponent;
  let saveBandIdSpy: ReturnType<typeof vi.fn>;

  beforeEach(async () => {
    saveBandIdSpy = vi.fn();
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BandDiscographyComponent],
      providers: [
        provideRouter([]),
        { provide: ReleaseStateService, useValue: { saveBandId: saveBandIdSpy } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(BandDiscographyComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('releases', mockReleases);
    componentRef.setInput('bandId', 'b1');
    fixture.detectChanges();
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('typeClass() returns correct class for each type', () => {
    expect(component.typeClass('Album')).toBe('type--album');
    expect(component.typeClass('EP')).toBe('type--ep');
    expect(component.typeClass('Demo')).toBe('type--demo');
    expect(component.typeClass('Split')).toBe('type--split');
    expect(component.typeClass('Compilation')).toBe('type--compilation');
    expect(component.typeClass('Unknown')).toBe('type--default');
  });

  it('accentClass() returns correct class for each type', () => {
    expect(component.accentClass('Album')).toBe('accent--album');
    expect(component.accentClass('EP')).toBe('accent--ep');
    expect(component.accentClass('Demo')).toBe('accent--demo');
    expect(component.accentClass('Split')).toBe('accent--split');
    expect(component.accentClass('Compilation')).toBe('accent--compilation');
    expect(component.accentClass('Unknown')).toBe('');
  });

  it('navigateToRelease() saves bandId and navigates to the release route', () => {
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    component.navigateToRelease('r1');

    expect(saveBandIdSpy).toHaveBeenCalledWith('b1');
    expect(navSpy).toHaveBeenCalledWith(['/releases', 'r1']);
  });

  it('should show release management controls only for an Admin', () => {
    expect(fixture.nativeElement.querySelector('[data-testid="add-release"]')).toBeNull();
    componentRef.setInput('isAdmin', true);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[data-testid="add-release"]')).toBeTruthy();
    expect(fixture.nativeElement.querySelector('[data-testid="edit-release-r1"]')).toBeTruthy();
  });
});
