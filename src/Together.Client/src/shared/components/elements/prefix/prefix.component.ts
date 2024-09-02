import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { BaseComponent } from '@/core/abstractions';

@Component({
  selector: 'together-prefix',
  standalone: true,
  imports: [NgClass],
  templateUrl: './prefix.component.html',
})
export class PrefixComponent extends BaseComponent {
  @Input()
  name = '';

  @Input()
  fontSize = '10px';

  @Input()
  foreground = '#000';

  @Input()
  background = '#FFF';
}
