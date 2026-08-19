import { DatePipe } from '@angular/common';
import { afterNextRender, ChangeDetectionStrategy, Component, computed, effect, ElementRef, input, output, signal, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReleaseFormat } from '../../../infrastructure/api/models';

export interface ReleaseModalForm {
  title: string;
  releaseType: 'Demo' | 'EP' | 'Album' | 'Split' | 'Compilation';
  releaseDate: string | null;
  year: number | null;
  labelText: string | null;
  formats: ReleaseFormat[];
}

@Component({
  selector: 'dmc-release-modal',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule],
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
    releaseDate: new FormControl('', [Validators.pattern(/^\d{2}\/\d{2}\/\d{4}$/)]),
    year: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    labelText: new FormControl('', [Validators.maxLength(300)]),
    formats: new FormControl<ReleaseFormat[]>([], { nonNullable: true }),
  });
  readonly formatOptions: { value: ReleaseFormat; label: string }[] = [
    { value: 'Vinyl7', label: '7" Vinyl' }, { value: 'Vinyl10', label: '10" Vinyl' }, { value: 'Vinyl12', label: '12" Vinyl' }, { value: 'VinylLatheCut', label: 'Vinyl Lathe Cut' }, { value: 'VinylOther', label: 'Vinyl (Other)' }, { value: 'CD', label: 'CD' }, { value: 'CDR', label: 'CD-R' }, { value: 'DVD', label: 'DVD' }, { value: 'Cassette', label: 'Cassette' }, { value: 'Digital', label: 'Digital' },
  ];
  readonly isCalendarOpen = signal(false);
  readonly calendarMonth = signal(new Date());
  readonly calendarDays = computed(() => this.buildCalendarDays(this.calendarMonth()));

  constructor() {
    effect(() => {
      const data = this.initialData();
      this.form.reset({ title: data?.title ?? '', releaseType: data?.releaseType ?? 'Album', releaseDate: this.toDateDisplayValue(data?.releaseDate), year: data?.year?.toString() ?? '', labelText: data?.labelText ?? '', formats: this.normalizeFormats(data?.formats ?? []) }, { emitEvent: false });
      this.setCalendarMonthFromValue(data?.releaseDate);
    });
    afterNextRender(() => this.titleInput()?.nativeElement.focus());
  }

  onSave(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.save.emit({ title: value.title.trim(), releaseType: value.releaseType, releaseDate: this.toIsoDate(value.releaseDate), year: value.year ? Number(value.year) : null, labelText: value.labelText?.trim() || null, formats: value.formats });
  }

  onReleaseDateInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const digits = input.value.replace(/\D/g, '').slice(0, 8);
    const displayValue = digits.length > 4 ? `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4)}` : digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits;
    this.form.controls.releaseDate.setValue(displayValue);
    const date = this.parseDisplayDate(displayValue);
    if (date) {
      this.calendarMonth.set(new Date(date.year, date.month - 1, 1));
    }
  }

  toggleCalendar(): void {
    this.isCalendarOpen.update(value => !value);
  }

  changeCalendarMonth(offset: number): void {
    const current = this.calendarMonth();
    this.calendarMonth.set(new Date(current.getFullYear(), current.getMonth() + offset, 1));
  }

  selectCalendarDate(date: Date): void {
    this.form.controls.releaseDate.setValue(this.toDisplayDate(date));
    this.isCalendarOpen.set(false);
  }

  isSelectedCalendarDate(date: Date): boolean {
    return this.toDisplayDate(date) === this.form.controls.releaseDate.value;
  }

  toggleFormat(format: ReleaseFormat): void {
    const formats = this.form.controls.formats.value;
    this.form.controls.formats.setValue(formats.includes(format) ? formats.filter(item => item !== format) : [...formats, format]);
  }

  isFormatSelected(format: ReleaseFormat): boolean {
    return this.form.controls.formats.value.includes(format);
  }

  formatsSummary(): string {
    const selected = this.form.controls.formats.value;
    return selected.length ? selected.map(format => this.formatOptions.find(option => option.value === format)?.label ?? format).join(', ') : 'Select formats';
  }

  private normalizeFormats(formats: readonly string[]): ReleaseFormat[] {
    return formats
      .map(format => this.formatOptions.find(option => option.value === format.trim() || option.label === format.trim())?.value)
      .filter((format): format is ReleaseFormat => format !== undefined);
  }

  private toDateDisplayValue(value: string | null | undefined): string {
    if (!value) {
      return '';
    }

    const match = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
    return match ? `${match[3]}/${match[2]}/${match[1]}` : '';
  }

  private toIsoDate(value: string | null): string | null {
    const date = this.parseDisplayDate(value ?? '');
    return date ? `${date.year}-${date.month.toString().padStart(2, '0')}-${date.day.toString().padStart(2, '0')}` : null;
  }

  private parseDisplayDate(value: string): { day: number; month: number; year: number } | null {
    const match = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(value);
    if (!match) {
      return null;
    }
    const day = Number(match[1]);
    const month = Number(match[2]);
    const year = Number(match[3]);
    const date = new Date(year, month - 1, day);
    return date.getFullYear() === year && date.getMonth() === month - 1 && date.getDate() === day ? { day, month, year } : null;
  }

  private setCalendarMonthFromValue(value: string | null | undefined): void {
    const displayValue = this.toDateDisplayValue(value);
    const date = this.parseDisplayDate(displayValue);
    this.calendarMonth.set(date ? new Date(date.year, date.month - 1, 1) : new Date());
  }

  private toDisplayDate(date: Date): string {
    return `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}`;
  }

  private buildCalendarDays(month: Date): (Date | null)[] {
    const firstDay = new Date(month.getFullYear(), month.getMonth(), 1);
    const daysBefore = firstDay.getDay();
    const lastDay = new Date(month.getFullYear(), month.getMonth() + 1, 0).getDate();
    return Array.from({ length: daysBefore + lastDay }, (_, index) => index < daysBefore ? null : new Date(month.getFullYear(), month.getMonth(), index - daysBefore + 1));
  }
}
