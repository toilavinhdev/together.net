import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { Button } from 'primeng/button';
import { NgIf } from '@angular/common';
import { Emoji } from '@ctrl/ngx-emoji-mart/ngx-emoji';

@Component({
  selector: 'together-emoji-picker',
  standalone: true,
  imports: [PickerComponent, Button, NgIf],
  templateUrl: './emoji-picker.component.html',
  styles: `
    .emoji-picker-container {
      position: relative;

      .emoji-mart {
        position: absolute;
        z-index: 3;
        bottom: 120%;
        transform: scale(0.8);
        right: 0;
        transform-origin: bottom right;
      }
    }
  `,
  encapsulation: ViewEncapsulation.Emulated,
})
export class EmojiPickerComponent {
  @ViewChild('emojiPickerContainer') container:
    | ElementRef<HTMLElement>
    | undefined;

  @Output() onSelectEmoji = new EventEmitter<string>();

  @Input() selectedFocusToTpl?: ElementRef;

  isOpened = false;

  set: Emoji['set'] = 'twitter';

  eventHandler = (event: Event) => {
    // Watching for outside clicks
    if (!this.container?.nativeElement.contains(event.target as Node)) {
      this.isOpened = false;
      window.removeEventListener('click', this.eventHandler);
      this.selectedFocusToTpl?.nativeElement.focus();
    }
  };

  toggle() {
    if (!this.container) {
      return;
    }
    this.isOpened = !this.isOpened;
    if (this.isOpened) {
      window.addEventListener('click', this.eventHandler);
    } else {
      window.removeEventListener('click', this.eventHandler);
    }
  }

  emojiSelected(event: any) {
    this.onSelectEmoji.emit(event.emoji.native);
    this.isOpened = false;
    this.selectedFocusToTpl?.nativeElement.focus();
  }
}
