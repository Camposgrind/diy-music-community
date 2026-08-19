import { afterNextRender, ChangeDetectionStrategy, Component, ElementRef, input, output, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BandWriteRequest, GenreModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-band-create-modal',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './band-create-modal.component.html',
  styleUrl: './band-create-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BandCreateModalComponent {
  readonly countries = input<string[]>([]);
  readonly genres = input<GenreModel[]>([]);
  readonly saving = input(false);
  readonly error = input<string | null>(null);

  readonly save = output<BandWriteRequest>();
  readonly close = output<void>();
  private readonly nameInput = viewChild<ElementRef<HTMLInputElement>>('nameInput');

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
    country: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    location: new FormControl('', { nonNullable: true }),
    formationYear: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    splitUpYear: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    genreId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    status: new FormControl<'Active' | 'SplitUp' | 'OnHold'>('Active', { nonNullable: true, validators: [Validators.required] }),
    musicUrlPortal: new FormControl('', { nonNullable: true }),
    bandContact: new FormControl('', { nonNullable: true }),
  });

  constructor() {
    afterNextRender(() => this.nameInput()?.nativeElement.focus());
  }

  onSave(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.save.emit({
      name: value.name.trim(),
      country: value.country,
      genreId: value.genreId,
      status: value.status,
      location: value.location.trim() || null,
      formationYear: value.formationYear ? Number(value.formationYear) : null,
      splitUpYear: value.status === 'SplitUp' ? Number(value.splitUpYear) : null,
      musicUrlPortal: value.musicUrlPortal.trim() || null,
      bandContact: value.bandContact.trim() || null,
    });
  }

  onStatusChange(): void {
    const splitUpYear = this.form.controls.splitUpYear;
    if (this.form.controls.status.value === 'SplitUp') {
      splitUpYear.setValidators([Validators.required, Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]);
    } else {
      splitUpYear.setValue('', { emitEvent: false });
      splitUpYear.setValidators([Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]);
    }
    splitUpYear.updateValueAndValidity({ emitEvent: false });
  }
}
