import { Component } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { NgClass } from '@angular/common';

@Component({
  selector: 'together-container',
  standalone: true,
  imports: [NgClass],
  templateUrl: './container.component.html',
})
export class ContainerComponent extends BaseComponent {}
