import { Component, Input } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';

@Component({
  selector: 'together-svg-icon',
  standalone: true,
  template: `
    <svg
      [class]="componentClass"
      [style.height.px]="height || size"
      [style.width.px]="width || size"
      xmlns="http://www.w3.org/2000/svg"
    >
      <use [attr.xlink:href]="iconUrl"></use>
    </svg>
  `,
})
export class SvgIconComponent extends BaseComponent {
  @Input()
  name!: string;

  @Input()
  size: number = 25;

  @Input()
  width?: number | string;

  @Input()
  height?: number | string;

  get iconUrl() {
    return `${window.location.href}#${this.name}`;
  }
}
