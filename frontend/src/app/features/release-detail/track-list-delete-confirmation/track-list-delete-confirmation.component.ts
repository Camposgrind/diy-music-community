import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'dmc-track-list-delete-confirmation',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './track-list-delete-confirmation.component.html',
  styleUrl: './track-list-delete-confirmation.component.scss',
})
export class TrackListDeleteConfirmationComponent {
  readonly releaseTitle = input.required<string>();
  readonly deleting = input(false);
  readonly error = input<string | null>(null);
  readonly confirm = output<void>();
  readonly close = output<void>();
}
