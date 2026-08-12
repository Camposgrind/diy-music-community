import { Component, inject, computed } from '@angular/core';
import { Router } from '@angular/router';
import { SearchStateService } from '../../home/search-state.service';

@Component({
  selector: 'dmc-back-to-results',
  standalone: true,
  imports: [],
  templateUrl: './back-to-results.component.html',
  styleUrl: './back-to-results.component.scss',
})
export class BackToResultsComponent {
  private readonly searchState = inject(SearchStateService);
  private readonly router = inject(Router);

  readonly hasState = computed(() => this.searchState.state() !== null);

  goBack(): void {
    if (this.searchState.state()) {
      this.router.navigate(['/']);
    } else {
      this.router.navigate(['/']);
    }
  }
}
