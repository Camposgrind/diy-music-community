import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { BandReleaseModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-release-delete-confirmation',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './release-delete-confirmation.component.html',
  styleUrl: './release-delete-confirmation.component.scss',
})
export class ReleaseDeleteConfirmationComponent {
  readonly release = input.required<BandReleaseModel>();
  readonly deleting = input(false);
  readonly error = input<string | null>(null);
  readonly confirm = output<void>();
  readonly close = output<void>();
}
