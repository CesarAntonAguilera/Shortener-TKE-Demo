import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { UrlShortenerService } from '../../services/url-shortener.service';
import { FullscreenLoaderComponent } from '../../shared/fullscreen-loader/fullscreen-loader.component';
import { ShortenUrlResponseDto } from '../../models/shortenUrlResponseDto.model';

@Component({
  selector: 'app-shortener',
  standalone: true,
  imports: [CommonModule, FormsModule, FullscreenLoaderComponent],
  templateUrl: './shortener.component.html',
  styleUrls: ['./shortener.component.scss'],
})
export class ShortenerComponent {
  longUrl = '';
  loading = false;

  result: ShortenUrlResponseDto | null = null;
  errorMessage: string | null = null;

  constructor(
    private api: UrlShortenerService,
    private cdr: ChangeDetectorRef
  ) {}

  shorten(): void {
    this.errorMessage = null;
    this.result = null;

    const value = this.longUrl.trim();
    if (!value) return;

    this.loading = true;

    this.api
      .shorten(value)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: (res) => {
          this.result = res;
          this.cdr.markForCheck();
        },
        error: (err) => {
          const detail = err?.error?.detail;
          const title = err?.error?.title;

          if (err?.status === 400) {
            this.errorMessage =
              detail || title || 'Invalid URL. It must be http or https.';
          } else {
            this.errorMessage = 'There was an unexpected error, please, try again.';
          }

          this.cdr.markForCheck();
        },
      });
  }

  async copy(text: string): Promise<void> {
    try {
      await navigator.clipboard.writeText(text);
    } catch {
      window.prompt('Copy the URL:', text);
    }
  }
}
