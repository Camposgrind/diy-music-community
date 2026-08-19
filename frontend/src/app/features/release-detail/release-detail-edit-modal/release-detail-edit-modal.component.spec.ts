import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { vi } from 'vitest';
import { ReleaseDetailEditModalComponent } from './release-detail-edit-modal.component';

describe('ReleaseDetailEditModalComponent', () => {
  let fixture: ComponentFixture<ReleaseDetailEditModalComponent>;
  let componentRef: ComponentRef<ReleaseDetailEditModalComponent>;
  let component: ReleaseDetailEditModalComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ReleaseDetailEditModalComponent] }).compileComponents();
    fixture = TestBed.createComponent(ReleaseDetailEditModalComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('initialData', {
      title: 'Scum',
      releaseType: 'Album',
      year: 1987,
      tracks: [{ title: 'You Suffer' }, { title: 'Multinational Corporations' }],
    });
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('preloads release main information and tracks', () => {
    expect(component.form.getRawValue()).toEqual({
      title: 'Scum',
      releaseType: 'Album',
      year: '1987',
      tracks: [{ title: 'You Suffer' }, { title: 'Multinational Corporations' }],
    });
  });

  it('emits tracks in their visual order without track numbers', () => {
    const save = vi.fn();
    component.save.subscribe(save);

    component.moveTrack(1, -1);
    component.addTrack();
    component.tracks.at(2).controls.title.setValue('Pollution');
    component.onSave();

    expect(save).toHaveBeenCalledWith({
      title: 'Scum',
      releaseType: 'Album',
      year: 1987,
      tracks: [
        { title: 'Multinational Corporations' },
        { title: 'You Suffer' },
        { title: 'Pollution' },
      ],
    });
  });

  it('does not emit while a track title is invalid', () => {
    const save = vi.fn();
    component.save.subscribe(save);
    component.addTrack();

    component.onSave();

    expect(save).not.toHaveBeenCalled();
    expect(component.tracks.at(2).controls.title.touched).toBe(true);
  });

  it('focuses the new track title after adding a track', () => {
    component.addTrack();
    fixture.detectChanges();

    expect(document.activeElement?.id).toBe('track-title-2');
  });

  it('removes a track from the submitted list', () => {
    const save = vi.fn();
    component.save.subscribe(save);

    component.removeTrack(0);
    component.onSave();

    expect(save).toHaveBeenCalledWith({
      title: 'Scum',
      releaseType: 'Album',
      year: 1987,
      tracks: [{ title: 'Multinational Corporations' }],
    });
  });

  it('should move and remove the track selected in the visible list', () => {
    const moveDown = fixture.nativeElement.querySelector('button[aria-label="Move track 1 down"]') as HTMLButtonElement;
    moveDown.click();
    fixture.detectChanges();

    expect(component.tracks.getRawValue()).toEqual([
      { title: 'Multinational Corporations' },
      { title: 'You Suffer' },
    ]);

    const removeFirst = fixture.nativeElement.querySelector('button[aria-label="Remove track 1"]') as HTMLButtonElement;
    removeFirst.click();
    fixture.detectChanges();

    expect(component.tracks.getRawValue()).toEqual([{ title: 'You Suffer' }]);
    expect((fixture.nativeElement.querySelector('#track-title-0') as HTMLInputElement).value).toBe('You Suffer');
  });
});
