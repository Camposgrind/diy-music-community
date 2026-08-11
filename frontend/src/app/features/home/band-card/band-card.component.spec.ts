import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { BandCardComponent } from './band-card.component';
import { BandListItemModel } from '../../../infrastructure/api/models';

describe('BandCardComponent', () => {
  let fixture: ComponentFixture<BandCardComponent>;
  let componentRef: ComponentRef<BandCardComponent>;
  let el: HTMLElement;

  const mockBand: BandListItemModel = {
    id: '1',
    name: 'Discharge',
    country: 'United Kingdom',
    genre: 'D-Beat',
    status: 'Active',
    formationYear: 1977,
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BandCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BandCardComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('band', mockBand);
    fixture.detectChanges();
    el = fixture.nativeElement;
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display band name', () => {
    expect(el.querySelector('.band-card__name')?.textContent).toContain('Discharge');
  });

  it('should display country', () => {
    expect(el.textContent).toContain('United Kingdom');
  });

  it('should display genre badge', () => {
    expect(el.querySelector('.band-card__genre-badge')?.textContent).toContain('D-Beat');
  });

  it('should display status', () => {
    expect(el.querySelector('.band-card__status')?.textContent?.trim()).toBe('Active');
  });

  it('should display formation year', () => {
    expect(el.textContent).toContain('1977');
  });

  it('should not display formation year when null', () => {
    componentRef.setInput('band', { ...mockBand, formationYear: null });
    fixture.detectChanges();
    const labels = el.querySelectorAll('.band-card__label');
    const formedLabel = Array.from(labels).find((l) => l.textContent?.includes('Formed'));
    expect(formedLabel).toBeFalsy();
  });

  it('should apply active status class', () => {
    const statusEl = el.querySelector('.band-card__status');
    expect(statusEl?.classList.contains('band-card__status--active')).toBe(true);
  });

  it('should not apply active status class for inactive band', () => {
    componentRef.setInput('band', { ...mockBand, status: 'Split-up' });
    fixture.detectChanges();
    const statusEl = el.querySelector('.band-card__status');
    expect(statusEl?.classList.contains('band-card__status--active')).toBe(false);
  });
});
