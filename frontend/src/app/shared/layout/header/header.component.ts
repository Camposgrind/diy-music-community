import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { SearchStateService } from '../../../features/home/search-state.service';

@Component({
  selector: 'dmc-header',
  standalone: true,
  imports: [],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly searchState = inject(SearchStateService);
  private readonly router = inject(Router);

  clearAndGoHome(): void {
    this.searchState.clear();
    this.router.navigate(['/']);
  }
}
