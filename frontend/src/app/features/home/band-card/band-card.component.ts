import { Component, input } from '@angular/core';
import { NgClass } from '@angular/common';
import { BandListItemModel } from '../../../infrastructure/api/models';
import { CountryFlagPipe } from '../../../shared/pipes/country-flag.pipe';

@Component({
  selector: 'dmc-band-card',
  standalone: true,
  imports: [CountryFlagPipe, NgClass],
  templateUrl: './band-card.component.html',
  styleUrl: './band-card.component.scss',
})
export class BandCardComponent {
  readonly band = input.required<BandListItemModel>();
}
