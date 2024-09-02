import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { TabMenuModule } from 'primeng/tabmenu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'together-setting',
  standalone: true,
  imports: [TabMenuModule],
  templateUrl: './setting.component.html',
})
export class SettingComponent extends BaseComponent implements OnInit {
  items: MenuItem[] = [
    {
      label: 'Thay đổi mật khẩu',
      routerLink: '/settings/update-password',
    },
  ];

  ngOnInit() {
    this.commonService.breadcrumb$.next([
      {
        title: 'Cài đặt',
      },
    ]);
  }
}
