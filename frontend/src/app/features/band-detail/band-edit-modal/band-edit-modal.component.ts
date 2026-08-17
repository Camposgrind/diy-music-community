import { afterNextRender, ChangeDetectionStrategy, Component, effect, ElementRef, input, output, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { GenreModel } from '../../../infrastructure/api/models';

export interface BandGeneralEditForm {
  name: string;
  country: string;
  location: string | null;
  formationYear: number | null;
  genreId: string;
  status: 'Active' | 'SplitUp' | 'OnHold';
  musicUrlPortal: string | null;
  bandContact: string | null;
}

@Component({
  selector: 'dmc-band-edit-modal',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './band-edit-modal.component.html',
  styleUrl: './band-edit-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BandEditModalComponent {
  readonly countries = input<string[]>([]);
  readonly genres = input<GenreModel[]>([]);
  readonly initialData = input.required<BandGeneralEditForm>();
  readonly saving = input(false);
  readonly error = input<string | null>(null);
  readonly save = output<BandGeneralEditForm>();
  readonly close = output<void>();
  private readonly nameInput = viewChild<ElementRef<HTMLInputElement>>('nameInput');

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
    country: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    location: new FormControl('', { nonNullable: true }),
    formationYear: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    genreId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    status: new FormControl<'Active' | 'SplitUp' | 'OnHold'>('Active', { nonNullable: true, validators: [Validators.required] }),
    musicUrlPortal: new FormControl('', { nonNullable: true }),
    bandContact: new FormControl('', { nonNullable: true }),
  });

  constructor() {
    effect(() => {
      const data = this.initialData();
      this.form.reset({
        ...data,
        location: data.location ?? '',
        formationYear: data.formationYear?.toString() ?? '',
        musicUrlPortal: data.musicUrlPortal ?? '',
        bandContact: data.bandContact ?? '',
      }, { emitEvent: false });
    });
    afterNextRender(() => this.nameInput()?.nativeElement.focus());
  }

  onSave(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.save.emit({
      name: value.name.trim(), country: value.country, genreId: value.genreId, status: value.status,
      location: value.location.trim() || null,
      formationYear: value.formationYear ? Number(value.formationYear) : null,
      musicUrlPortal: value.musicUrlPortal.trim() || null,
      bandContact: value.bandContact.trim() || null,
    });
  }
}
