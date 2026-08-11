import { Component, input, output } from '@angular/core';
import { BandListItemModel, PagedResult } from '../../../infrastructure/api/models';
import { BandCardComponent } from '../band-card/band-card.component';

@Component({
  selector: 'dmc-bands-results',
  standalone: true,
  imports: [BandCardComponent],
  templateUrl: './bands-results.component.html',
  styleUrl: './bands-results.component.scss',
})
export class BandsResultsComponent {
  readonly results = input<PagedResult<BandListItemModel> | null>(null);
  readonly loading = input(false);
  readonly error = input<string | null>(null);
  readonly pageChange = output<number>();

  get totalPages(): number {
    const r = this.results();
    if (!r || r.pageSize === 0) return 0;
    return Math.ceil(r.totalCount / r.pageSize);
  }

  get pages(): number[] {
    const total = this.totalPages;
    const current = this.results()?.page ?? 1;
    const pages: number[] = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  onPageChange(page: number): void {
    this.pageChange.emit(page);
  }
}
