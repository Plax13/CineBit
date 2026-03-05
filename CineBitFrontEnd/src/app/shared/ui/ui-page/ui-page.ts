import { Component } from '@angular/core';
import { Input } from '@angular/core';
@Component({
  selector: 'ui-page',
  imports: [],
  templateUrl: './ui-page.html',
  styleUrl: './ui-page.css',
})
export class UiPage {
      @Input() showBrand = true; // se in una pagina non lo vuoi
}
