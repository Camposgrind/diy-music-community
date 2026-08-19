import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { ReleaseHeroComponent } from './release-hero.component';
import { ReleaseDetailModel } from '../../../infrastructure/api/models';

const mockRelease: ReleaseDetailModel = {
  id: 'r1',
  title: 'Scum',
  releaseType: 'Album',
  releaseDate: '1987-07-01',
  year: 1987,
  labelText: 'Earache Records',
  coverImageUrl: null,
  band: null,
  formats: ['Vinyl'],
  tracks: [],
};

describe('ReleaseHeroComponent', () => {
  let fixture: ComponentFixture<ReleaseHeroComponent>;
  let componentRef: ComponentRef<ReleaseHeroComponent>;
  let component: ReleaseHeroComponent;

  beforeEach(async () => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [ReleaseHeroComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ReleaseHeroComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('release', mockRelease);
    fixture.detectChanges();
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('lightboxUrl is null initially', () => {
    expect(component.lightboxUrl()).toBeNull();
  });

  it('isCustomImage() returns false for null', () => {
    expect(component.isCustomImage(null)).toBe(false);
  });

  it('isCustomImage() returns false for undefined', () => {
    expect(component.isCustomImage(undefined)).toBe(false);
  });

  it('isCustomImage() returns false for the fallback image', () => {
    expect(component.isCustomImage(component.fallback)).toBe(false);
  });

  it('isCustomImage() returns true for a real URL', () => {
    expect(component.isCustomImage('https://example.com/cover.jpg')).toBe(true);
  });

  it('openLightbox() sets lightboxUrl when image is custom', () => {
    component.openLightbox('https://example.com/cover.jpg');
    expect(component.lightboxUrl()).toBe('https://example.com/cover.jpg');
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
    component.openLightbox('https://example.com/cover.jpg');
    component.closeLightbox();
    expect(component.lightboxUrl()).toBeNull();
  });

  it('onImgError() replaces src with fallback', () => {
    const img = document.createElement('img');
    img.src = 'https://broken.url/cover.jpg';
    component.onImgError({ target: img } as unknown as Event);
    expect(img.src).toContain(component.fallback);
  });

  it('typeClass() returns a lowercase type class string', () => {
    expect(component.typeClass('Album')).toBe('type--album');
    expect(component.typeClass('EP')).toBe('type--ep');
  });

  it('shows release edit and delete controls only for administrators', () => {
    const editDetails = vi.fn();
    const deleteRelease = vi.fn();
    component.editDetails.subscribe(editDetails);
    component.deleteRelease.subscribe(deleteRelease);
    componentRef.setInput('isAdmin', true);
    fixture.detectChanges();

    const detailsButton = fixture.nativeElement.querySelector('[data-testid="edit-release-details"]') as HTMLButtonElement;
    const deleteButton = fixture.nativeElement.querySelector('[data-testid="delete-release"]') as HTMLButtonElement;
    expect(detailsButton).toBeTruthy();
    expect(deleteButton).toBeTruthy();
    detailsButton.click();
    deleteButton.click();
    expect(editDetails).toHaveBeenCalledOnce();
    expect(deleteRelease).toHaveBeenCalledOnce();

    componentRef.setInput('isAdmin', false);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[data-testid="edit-release-details"]')).toBeNull();
  });
});
