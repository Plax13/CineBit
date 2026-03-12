import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-ai-loader',
  imports: [],
  templateUrl: './ai-loader.html',
  styleUrl: './ai-loader.css',
})

export class AiLoader {
  @Input() text: string = 'Loading...';
}
