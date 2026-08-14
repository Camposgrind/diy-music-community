import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';
import { signal } from '@angular/core';
import { Router } from '@angular/router';
import { BackToResultsComponent } from './back-to-results.component';
import { SearchStateService } from '../../home/search-state.service';

describe('BackToResultsComponent', () => {
  let fixture: ComponentFixture<BackToResultsComponent>;
  let component: BackToResultsComponent;

  const setup = async (state: object | null) => {
    const stateSig = signal(state);
    const fakeSearch = { state: stateSig };

    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BackToResultsComponent],
      providers: [
        provideRouter([]),
        { provide: SearchStateService, useValue: fakeSearch },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(BackToResultsComponent);
    fixture.detectChanges();
    component = fixture.componentInstance;
    return { stateSig };
  };

  it('should create', async () => {
    await setup(null);
    expect(component).toBeTruthy();
  });

  it('hasState is false when search state is null', async () => {
    await setup(null);
    expect(component.hasState()).toBe(false);
  });

  it('hasState is true when search state is present', async () => {
    await setup({ query: {}, page: 1 });
    expect(component.hasState()).toBe(true);
  });

  it('goBack() navigates to / when state exists', async () => {
    await setup({ query: {}, page: 1 });
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    component.goBack();
    expect(navSpy).toHaveBeenCalledWith(['/']);
  });

  it('goBack() navigates to / when state is null', async () => {
    await setup(null);
    const router = TestBed.inject(Router);
    const navSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    component.goBack();
    expect(navSpy).toHaveBeenCalledWith(['/']);
  });
});
