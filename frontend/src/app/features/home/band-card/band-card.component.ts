import { Component, input } from '@angular/core';
import { BandListItemModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-band-card',
  standalone: true,
  templateUrl: './band-card.component.html',
  styleUrl: './band-card.component.scss',
})
export class BandCardComponent {
  readonly band = input.required<BandListItemModel>();
}
