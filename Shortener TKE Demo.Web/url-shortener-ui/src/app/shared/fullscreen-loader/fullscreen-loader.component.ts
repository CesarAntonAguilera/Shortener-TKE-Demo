import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-fullscreen-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fullscreen-loader.component.html',
  styleUrls: ['./fullscreen-loader.component.scss'],
})
export class FullscreenLoaderComponent {
  @Input() visible = false;
  @Input() text = 'Loading...';
}
