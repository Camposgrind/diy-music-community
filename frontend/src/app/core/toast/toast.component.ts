import { Component, inject } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'dmc-toast',
  standalone: true,
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="toast toast--{{ toast.type }}"
          role="alert"
          (click)="toastService.dismiss(toast.id)">
          <span class="toast__icon-wrap">
            @if (toast.type === 'error') { ✕ }
            @else if (toast.type === 'success') { ✓ }
            @else { ℹ }
          </span>
          <span class="toast__body">
            <span class="toast__label">
              @if (toast.type === 'error') { Error }
              @else if (toast.type === 'success') { Success }
              @else { Info }
            </span>
            <span class="toast__message">{{ toast.message }}</span>
          </span>
          <span class="toast__close">×</span>
          <span class="toast__progress"></span>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: 5rem;
      left: 50%;
      transform: translateX(-50%);
      z-index: 10000;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.75rem;
      pointer-events: none;
      width: max-content;
    }

    .toast {
      position: relative;
      display: flex;
      align-items: center;
      gap: 0.875rem;
      overflow: hidden;
      border-radius: 6px;
      padding: 0.9rem 1.25rem;
      min-width: 340px;
      max-width: 480px;
      cursor: pointer;
      pointer-events: all;
      animation: toast-in 0.35s cubic-bezier(0.175, 0.885, 0.32, 1.275) both;
      transform-origin: top center;

      &--error {
        background: #7a0000;
        border: 1px solid #b30000;
        box-shadow: 0 4px 0 #3d0000, 0 8px 28px rgba(180, 0, 0, 0.55), inset 0 1px 0 rgba(255,255,255,0.08);
      }

      &--success {
        background: #1b5e20;
        border: 1px solid #2e7d32;
        box-shadow: 0 4px 0 #0a2f0d, 0 8px 28px rgba(46, 125, 50, 0.55), inset 0 1px 0 rgba(255,255,255,0.08);
      }

      &--info {
        background: #0d3b6e;
        border: 1px solid #1565c0;
        box-shadow: 0 4px 0 #051e3e, 0 8px 28px rgba(21, 101, 192, 0.55), inset 0 1px 0 rgba(255,255,255,0.08);
      }

      &__icon-wrap {
        flex-shrink: 0;
        width: 2.1rem;
        height: 2.1rem;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.9rem;
        font-weight: 900;
        background: rgba(0, 0, 0, 0.25);
        color: #fff;
        box-shadow: 0 0 10px rgba(0,0,0,0.4);
      }

      &__body {
        flex: 1;
        display: flex;
        flex-direction: column;
        gap: 0.1rem;
        min-width: 0;
      }

      &__label {
        font-family: 'Oswald', sans-serif;
        font-size: 0.65rem;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: 2px;
        color: rgba(255, 255, 255, 0.6);
      }

      &__message {
        font-size: 0.875rem;
        font-weight: 500;
        color: #fff;
        line-height: 1.4;
        word-break: break-word;
      }

      &__close {
        flex-shrink: 0;
        font-size: 1.3rem;
        color: rgba(255, 255, 255, 0.45);
        line-height: 1;
        align-self: flex-start;
        padding: 0 0.1rem;
        transition: color 0.15s;

        &:hover { color: #fff; }
      }

      &__progress {
        position: absolute;
        bottom: 0;
        left: 0;
        height: 3px;
        width: 100%;
        animation: toast-progress 4s linear forwards;
        transform-origin: left;
        background: rgba(255, 255, 255, 0.35);
      }
    }

    @keyframes toast-in {
      from { opacity: 0; transform: translateY(-0.75rem) scale(0.95); }
      to   { opacity: 1; transform: translateY(0) scale(1); }
    }

    @keyframes toast-progress {
      from { transform: scaleX(1); }
      to   { transform: scaleX(0); }
    }
  `],
})
export class ToastComponent {
  readonly toastService = inject(ToastService);
}

