import { Component, input } from '@angular/core';
import { BandMemberModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-band-members',
  standalone: true,
  imports: [],
  templateUrl: './band-members.component.html',
  styleUrl: './band-members.component.scss',
})
export class BandMembersComponent {
  readonly members = input.required<BandMemberModel[]>();
  readonly title = input.required<string>();
}
