import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComponentRef } from '@angular/core';
import { ReleaseTracksComponent } from './release-tracks.component';
import { ReleaseTrackModel } from '../../../infrastructure/api/models';

const mockTracks: ReleaseTrackModel[] = [
  { releaseId: 'r1', title: 'You Suffer', trackNumber: 1 },
  { releaseId: 'r1', title: 'Mass Appeal Madness', trackNumber: 3 },
  { releaseId: 'r1', title: 'Instinct of Survival', trackNumber: 2 },
];

describe('ReleaseTracksComponent', () => {
  let fixture: ComponentFixture<ReleaseTracksComponent>;
  let componentRef: ComponentRef<ReleaseTracksComponent>;
  let component: ReleaseTracksComponent;

  beforeEach(async () => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [ReleaseTracksComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ReleaseTracksComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('tracks', mockTracks);
    fixture.detectChanges();
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('sortedTracks() returns tracks ordered by trackNumber ascending', () => {
    const sorted = component.sortedTracks();
    expect(sorted[0].trackNumber).toBe(1);
    expect(sorted[1].trackNumber).toBe(2);
    expect(sorted[2].trackNumber).toBe(3);
  });

  it('sortedTracks() does not mutate the original input', () => {
    component.sortedTracks();
    expect(component.tracks()[0].trackNumber).toBe(1);
    expect(component.tracks()[1].trackNumber).toBe(3);
    expect(component.tracks()[2].trackNumber).toBe(2);
  });

  it('sortedTracks() returns an empty array when tracks is empty', async () => {
    componentRef.setInput('tracks', []);
    fixture.detectChanges();
    expect(component.sortedTracks()).toEqual([]);
  });
});
