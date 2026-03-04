import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

type Variant = 'primary' | 'secondary';
type BtnType = 'button' | 'submit' | 'reset';

@Component({
  selector: 'ui-button',
  standalone: true,
  templateUrl: './ui-button.html',
  imports: [RouterLink,]
})
export class UiButton {
  @Input() variant: Variant = 'primary';
  @Input() type: BtnType = 'button';
  @Input() disabled = false;

  get classes(): string {
    const base =
      'inline-flex items-center justify-center w-full h-11 rounded-full ' +
      'text-sm font-medium transition active:scale-[0.99] ' +
      'disabled:opacity-50 disabled:pointer-events-none';

    const primary = 'bg-[#55BBEF] text-black';
    const secondary = 'bg-transparent text-white border border-white';

    return `${base} ${this.variant === 'primary' ? primary : secondary}`;
  }
}