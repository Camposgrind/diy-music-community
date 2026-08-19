import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { BandDetailModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-band-delete-confirmation',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './band-delete-confirmation.component.html',
  styleUrl: './band-delete-confirmation.component.scss',
})
export class BandDeleteConfirmationComponent {
  readonly band = input.required<BandDetailModel>();
  readonly deleting = input(false);
  readonly error = input<string | null>(null);
  readonly confirm = output<void>();
  readonly close = output<void>();
}
