import { Component } from '@angular/core';
import { ClickOutsideDirective } from '@/shared/directives';
import { BaseComponent } from '@/core/abstractions';

@Component({
  selector: 'together-language-switch-modal',
  standalone: true,
  imports: [ClickOutsideDirective],
  templateUrl: './language-switch-modal.component.html',
})
export class LanguageSwitchModalComponent extends BaseComponent {
  items = [
    {
      title: 'Tiếng Việt',
      value: 'vi-VN',
      iconPath: 'shared/assets/images/language-vietnamese.png',
    },
    {
      title: 'English',
      value: 'en-US',
      iconPath: 'shared/assets/images/language-english.png',
    },
  ];

  visible = false;

  show() {
    setTimeout(() => {
      this.visible = true;
    }, 100);
  }

  onClickOutside() {
    this.visible = false;
  }

  switchLanguage(lang: string) {
    this.commonService.setLanguage(lang);
    this.visible = false;
    location.reload();
  }
}
