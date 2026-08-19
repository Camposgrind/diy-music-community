import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { BandMemberModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-member-delete-confirmation',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './member-delete-confirmation.component.html',
  styleUrl: './member-delete-confirmation.component.scss',
})
export class MemberDeleteConfirmationComponent {
  readonly member = input.required<BandMemberModel>();
  readonly deleting = input(false);
  readonly error = input<string | null>(null);
  readonly confirm = output<void>();
  readonly close = output<void>();
}
