import { afterNextRender, ChangeDetectionStrategy, Component, effect, ElementRef, input, output, viewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';

export type MemberType = 'current' | 'past';

export interface MemberModalForm {
  name: string;
  instrument: string | null;
  startYear: number | null;
  endYear: number | null;
  memberType: MemberType;
}

function yearOrderValidator(control: AbstractControl): ValidationErrors | null {
  const startYear = Number(control.get('startYear')?.value);
  const endYear = Number(control.get('endYear')?.value);
  return startYear && endYear && endYear < startYear ? { yearOrder: true } : null;
}

@Component({
  selector: 'dmc-member-modal', standalone: true, imports: [ReactiveFormsModule],
  templateUrl: './member-modal.component.html', styleUrl: './member-modal.component.scss', changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MemberModalComponent {
  readonly mode = input.required<'create' | 'edit'>();
  readonly memberType = input.required<MemberType>();
  readonly initialData = input<MemberModalForm | null>(null);
  readonly saving = input(false);
  readonly error = input<string | null>(null);
  readonly save = output<MemberModalForm>();
  readonly close = output<void>();
  private readonly nameInput = viewChild<ElementRef<HTMLInputElement>>('nameInput');

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
    instrument: new FormControl('', { nonNullable: true }),
    startYear: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    endYear: new FormControl('', [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]),
    memberType: new FormControl<MemberType>('current', { nonNullable: true }),
  }, { validators: yearOrderValidator });

  constructor() {
    effect(() => {
      const data = this.initialData();
      const type = data?.memberType ?? this.memberType();
      this.form.reset({ name: data?.name ?? '', instrument: data?.instrument ?? '', startYear: data?.startYear?.toString() ?? '', endYear: data?.endYear?.toString() ?? '', memberType: type }, { emitEvent: false });
      this.applyMemberType(type);
    });
    afterNextRender(() => this.nameInput()?.nativeElement.focus());
  }

  onMemberTypeChange(): void {
    this.applyMemberType(this.form.controls.memberType.value);
  }

  onSave(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.save.emit({
      name: value.name.trim(), instrument: value.instrument.trim() || null,
      startYear: value.startYear ? Number(value.startYear) : null,
      endYear: value.memberType === 'past' ? Number(value.endYear) : null,
      memberType: value.memberType,
    });
  }

  private applyMemberType(type: MemberType): void {
    const endYear = this.form.controls.endYear;
    endYear.setValidators(type === 'past'
      ? [Validators.required, Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]
      : [Validators.min(1000), Validators.max(new Date().getFullYear()), Validators.pattern(/^\d{4}$/)]);
    if (type === 'current') endYear.setValue('', { emitEvent: false });
    endYear.updateValueAndValidity({ emitEvent: false });
    this.form.updateValueAndValidity({ emitEvent: false });
  }
}
