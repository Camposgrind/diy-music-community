import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { provideRouter } from '@angular/router';
import { BandHeroComponent } from './band-hero.component';
import { BandDetailModel } from '../../../infrastructure/api/models';

const mockBand: BandDetailModel = {
  id: 'b1',
  name: 'Napalm Death',
  country: 'United Kingdom',
  location: 'Birmingham',
  status: 'Active',
  genre: 'Grindcore',
  formationYear: 1981,
  description: 'Legendary grindcore band.',
  logoImageUrl: null,
  bandImageUrl: null,
  musicUrlPortal: null,
  bandContact: null,
  releases: [],
  members: [],
};

describe('BandHeroComponent', () => {
  let fixture: ComponentFixture<BandHeroComponent>;
  let componentRef: ComponentRef<BandHeroComponent>;
  let component: BandHeroComponent;

  beforeEach(async () => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BandHeroComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(BandHeroComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('band', mockBand);
    fixture.detectChanges();
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('isCustomImage() returns false for null', () => {
    expect(component.isCustomImage(null)).toBe(false);
  });

  it('isCustomImage() returns false for the fallback image', () => {
    expect(component.isCustomImage(component.fallback)).toBe(false);
  });

  it('isCustomImage() returns true for a real URL', () => {
    expect(component.isCustomImage('https://example.com/img.jpg')).toBe(true);
  });

  it('openLightbox() sets lightboxUrl when image is custom', () => {
    component.openLightbox('https://example.com/img.jpg');
    expect(component.lightboxUrl()).toBe('https://example.com/img.jpg');
  });

  it('openLightbox() does NOT set lightboxUrl for fallback image', () => {
    component.openLightbox(component.fallback);
    expect(component.lightboxUrl()).toBeNull();
  });

  it('openLightbox() does NOT set lightboxUrl for null', () => {
    component.openLightbox(null);
    expect(component.lightboxUrl()).toBeNull();
  });

  it('closeLightbox() resets lightboxUrl to null', () => {
    component.openLightbox('https://example.com/img.jpg');
    component.closeLightbox();
    expect(component.lightboxUrl()).toBeNull();
  });

  it('onImgError() replaces src with fallback', () => {
    const img = document.createElement('img');
    img.src = 'https://broken.url/img.jpg';
    component.onImgError({ target: img } as unknown as Event);
    expect(img.src).toContain(component.fallback);
  });
});
