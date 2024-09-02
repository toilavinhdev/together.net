import { Component } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { AsyncPipe, NgIf } from '@angular/common';

@Component({
  selector: 'together-spinner',
  standalone: true,
  imports: [ProgressSpinnerModule, NgIf, AsyncPipe],
  templateUrl: './spinner.component.html',
})
export class SpinnerComponent extends BaseComponent {
  spinning$ = this.commonService.spinning$;

  constructor() {
    super();
  }
}
