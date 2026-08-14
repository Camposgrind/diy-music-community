import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { SearchStateService } from '../../../features/home/search-state.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'dmc-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly searchState = inject(SearchStateService);
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);

  clearAndGoHome(): void {
    this.searchState.clear();
    this.router.navigate(['/']);
  }

  logout(): void {
    this.auth.logout();
  }
}
