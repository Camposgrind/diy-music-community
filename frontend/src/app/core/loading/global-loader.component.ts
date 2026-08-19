import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LoadingService } from './loading.service';

@Component({
  selector: 'dmc-global-loader',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `@if (loading.isLoading() > 0) { <div class="global-loader" role="status" aria-live="polite" aria-label="Loading"><div class="global-loader__surface"><div class="global-loader__animation"><img src="/images/loading.gif?v=2" alt="" /></div><span>Loading</span></div></div> }`,
  styleUrl: './global-loader.component.scss',
})
export class GlobalLoaderComponent {
  readonly loading = inject(LoadingService);
}
