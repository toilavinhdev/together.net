import { Component } from '@angular/core';
import { ScrollTopModule } from 'primeng/scrolltop';

@Component({
  selector: 'together-scroll-top',
  standalone: true,
  imports: [ScrollTopModule],
  templateUrl: './scroll-top.component.html',
})
export class ScrollTopComponent {}
