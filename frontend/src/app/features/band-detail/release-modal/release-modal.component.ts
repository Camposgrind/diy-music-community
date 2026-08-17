import { afterNextRender, ChangeDetectionStrategy, Component, effect, ElementRef, input, output, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

export interface ReleaseModalForm {
  title: string;
  releaseType: 'Demo' | 'EP' | 'Album' | 'Split' | 'Compilation';
  year: number | null;
}

@Component({
  selector: 'dmc-release-modal',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './release-modal.component.html',
  styleUrl: './release-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReleaseModalComponent {
  readonly mode = input.required<'create' | 'edit'>();
  readonly initialData = input<ReleaseModalForm | null>(null);
  readonly saving = input(false);
  readonly error = input<string | null>(null);
  readonly save = output<ReleaseModalForm>();
  readonly close = output<void>();
  private readonly titleInput = viewChild<ElementRef<HTMLInputElement>>('titleInput');

  readonly form = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(300)] }),
    releaseType: new FormControl<ReleaseModalForm['releaseType']>('Album', { nonNullable: true, validators: [Validators.required] }),
    year: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
  });

  constructor() {
    effect(() => {
      const data = this.initialData();
      this.form.reset({ title: data?.title ?? '', releaseType: data?.releaseType ?? 'Album', year: data?.year?.toString() ?? '' }, { emitEvent: false });
    });
    afterNextRender(() => this.titleInput()?.nativeElement.focus());
  }

  onSave(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.save.emit({ title: value.title.trim(), releaseType: value.releaseType, year: value.year ? Number(value.year) : null });
  }
}
