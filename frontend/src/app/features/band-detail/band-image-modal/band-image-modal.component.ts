import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';
import { ImageUploadType } from '../../../infrastructure/api/models';

const MAX_IMAGE_SIZE_BYTES = 5 * 1024 * 1024;
const ALLOWED_EXTENSIONS = ['png', 'jpg', 'jpeg'];
const ALLOWED_MIME_TYPES = ['image/png', 'image/jpeg'];

@Component({
  selector: 'dmc-band-image-modal', standalone: true, changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './band-image-modal.component.html', styleUrl: './band-image-modal.component.scss',
})
export class BandImageModalComponent {
  readonly imageType = input.required<ImageUploadType>();
  readonly currentImageUrl = input<string | null>(null);
  readonly saving = input(false);
  readonly save = output<File>();
  readonly close = output<void>();
  readonly file = signal<File | null>(null);
  readonly previewUrl = signal<string | null>(null);
  readonly error = signal<string | null>(null);

  get title(): string { const label = this.imageType() === 'BandPhoto' ? 'Band Photo' : this.imageType() === 'BandLogo' ? 'Band Logo' : 'Release Cover'; return `${this.currentImageUrl() ? 'Edit' : 'Add'} ${label}`; }

  selectFile(event: Event): void { this.setFile((event.target as HTMLInputElement).files?.[0] ?? null); }
  onDrop(event: DragEvent): void { event.preventDefault(); this.setFile(event.dataTransfer?.files?.[0] ?? null); }
  onDragOver(event: DragEvent): void { event.preventDefault(); }
  submit(): void { if (!this.file()) { this.error.set('Select an image before saving.'); return; } this.save.emit(this.file()!); }
  cancel(): void { if (!this.saving()) { this.clearPreview(); this.close.emit(); } }

  private setFile(file: File | null): void {
    this.clearPreview(); this.error.set(null);
    if (!file) { return; }
    const extension = file.name.split('.').pop()?.toLowerCase();
    if (!extension || !ALLOWED_EXTENSIONS.includes(extension)) { this.error.set('Only PNG, JPG, and JPEG files are allowed.'); return; }
    if (!ALLOWED_MIME_TYPES.includes(file.type)) { this.error.set('The selected file is not a supported image.'); return; }
    if (file.size > MAX_IMAGE_SIZE_BYTES) { this.error.set('The image must be 5 MB or smaller.'); return; }
    this.file.set(file); this.previewUrl.set(URL.createObjectURL(file));
  }
  private clearPreview(): void { const url = this.previewUrl(); if (url) { URL.revokeObjectURL(url); } this.previewUrl.set(null); this.file.set(null); }
}
