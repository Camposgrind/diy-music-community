import { ChangeDetectionStrategy, Component, effect, ElementRef, input, output, signal, viewChildren } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

type ReleaseType = 'Demo' | 'EP' | 'Album' | 'Split' | 'Compilation';

export interface ReleaseDetailEditForm {
  title: string;
  releaseType: ReleaseType;
  year: number | null;
  tracks: { title: string }[];
}

type TrackForm = FormGroup<{ title: FormControl<string> }>;

@Component({
  selector: 'dmc-release-detail-edit-modal',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './release-detail-edit-modal.component.html',
  styleUrl: './release-detail-edit-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReleaseDetailEditModalComponent {
  readonly initialData = input<ReleaseDetailEditForm | null>(null);
  readonly tracksOnly = input(false);
  readonly saving = input(false);
  readonly error = input<string | null>(null);
  readonly save = output<ReleaseDetailEditForm>();
  readonly close = output<void>();
  private readonly trackInputs = viewChildren<ElementRef<HTMLInputElement>>('trackInput');
  private readonly pendingFocusIndex = signal<number | null>(null);

  readonly form = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(300)] }),
    releaseType: new FormControl<ReleaseType>('Album', { nonNullable: true, validators: [Validators.required] }),
    year: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    tracks: new FormArray<TrackForm>([]),
  });

  get tracks(): FormArray<TrackForm> {
    return this.form.controls.tracks;
  }

  constructor() {
    effect(() => {
      const data = this.initialData();
      this.form.controls.title.setValue(data?.title ?? '', { emitEvent: false });
      this.form.controls.releaseType.setValue(data?.releaseType ?? 'Album', { emitEvent: false });
      this.form.controls.year.setValue(data?.year?.toString() ?? '', { emitEvent: false });
      this.tracks.clear({ emitEvent: false });
      for (const track of data?.tracks ?? []) {
        this.tracks.push(this.createTrack(track.title), { emitEvent: false });
      }
      this.form.markAsPristine();
      this.form.markAsUntouched();
    });
    effect(() => {
      const index = this.pendingFocusIndex();
      const input = index === null ? undefined : this.trackInputs()[index];
      if (input) {
        input.nativeElement.focus();
        this.pendingFocusIndex.set(null);
      }
    });
  }

  addTrack(): void {
    this.tracks.push(this.createTrack(''));
    this.pendingFocusIndex.set(this.tracks.length - 1);
  }

  removeTrack(index: number): void {
    this.tracks.removeAt(index);
  }

  moveTrack(index: number, offset: -1 | 1): void {
    const targetIndex = index + offset;
    if (targetIndex < 0 || targetIndex >= this.tracks.length) {
      return;
    }
    const track = this.tracks.at(index);
    this.tracks.removeAt(index, { emitEvent: false });
    this.tracks.insert(targetIndex, track);
  }

  onSave(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.save.emit({
      title: value.title.trim(),
      releaseType: value.releaseType,
      year: value.year ? Number(value.year) : null,
      tracks: value.tracks.map((track) => ({ title: track.title.trim() })),
    });
  }

  private createTrack(title: string): TrackForm {
    return new FormGroup({
      title: new FormControl(title, { nonNullable: true, validators: [Validators.required, Validators.maxLength(300)] }),
    });
  }
}
