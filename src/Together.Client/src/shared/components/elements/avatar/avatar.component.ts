import { Component, Input } from '@angular/core';
import { AvatarModule } from 'primeng/avatar';
import { BaseComponent } from '@/core/abstractions';
import { NgClass } from '@angular/common';

@Component({
  selector: 'together-avatar',
  standalone: true,
  imports: [AvatarModule, NgClass],
  templateUrl: './avatar.component.html',
})
export class AvatarComponent extends BaseComponent {
  @Input()
  src: string | undefined = undefined;

  @Input()
  size = 28;
}
