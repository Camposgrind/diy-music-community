import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { signal } from '@angular/core';
import { vi } from 'vitest';
import { BackToBandComponent } from './back-to-band.component';
import { ReleaseStateService } from '../release-state.service';

describe('BackToBandComponent', () => {
  let fixture: ComponentFixture<BackToBandComponent>;
  let component: BackToBandComponent;

  const setup = async (bandId: string | null) => {
    const bandIdSig = signal(bandId);
    const fakeReleaseState = { bandId: bandIdSig };

    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BackToBandComponent],
      providers: [
        provideRouter([]),
        { provide: ReleaseStateService, useValue: fakeReleaseState },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(BackToBandComponent);
    fixture.detectChanges();
    component = fixture.componentInstance;
  };

  it('should create', async () => {
    await setup(null);
    expect(component).toBeTruthy();
  });

  it('hasBand is false when bandId is null', async () => {
    await setup(null);
    expect(component.hasBand()).toBe(false);
  });

  it('hasBand is true when bandId is set', async () => {
    await setup('band-1');
    expect(component.hasBand()).toBe(true);
  });

  it('goBack() navigates to /bands/:id when bandId is set', async () => {
    await setup('band-42');
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    component.goBack();
    expect(navSpy).toHaveBeenCalledWith(['/bands', 'band-42']);
  });

  it('goBack() navigates to / when bandId is null', async () => {
    await setup(null);
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    component.goBack();
    expect(navSpy).toHaveBeenCalledWith(['/']);
  });
});
