import { Component, input, computed } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'dmc-status-badge',
  standalone: true,
  imports: [NgClass],
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss',
})
export class StatusBadgeComponent {
  readonly status = input.required<string>();

  readonly label = computed(() => {
    switch (this.status()) {
      case 'SplitUp': return 'Split-Up';
      case 'OnHold': return 'On Hold';
      default: return this.status();
    }
  });

  readonly modifier = computed(() => {
    switch (this.status()) {
      case 'Active': return 'active';
      case 'SplitUp': return 'splitup';
      case 'OnHold': return 'onhold';
      default: return 'unknown';
    }
  });
}
