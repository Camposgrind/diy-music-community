import { Component, input, output } from '@angular/core';
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
  readonly memberType = input.required<'current' | 'past'>();
  readonly isAdmin = input(false);
  readonly addMember = output<'current' | 'past'>();
  readonly editMember = output<BandMemberModel>();
}
