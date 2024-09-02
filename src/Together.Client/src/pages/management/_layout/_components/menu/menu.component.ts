import { Component, OnInit } from '@angular/core';
import { NgForOf } from '@angular/common';
import { MenuItem } from 'primeng/api';
import { BaseComponent } from '@/core/abstractions';
import { MenuModule } from 'primeng/menu';
import { Ripple } from 'primeng/ripple';
import { TranslateModule } from '@ngx-translate/core';
import { UserService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-menu',
  standalone: true,
  imports: [NgForOf, MenuModule, Ripple, TranslateModule],
  templateUrl: './menu.component.html',
})
export class MenuComponent extends BaseComponent implements OnInit {
  items: MenuItem[] = [
    {
      label: 'Trang chủ',
      items: [
        {
          id: policies.Management.ViewDashboard,
          label: 'Thống kê diễn đàn',
          icon: 'pi pi-home',
          routerLink: '/management',
          visible: false,
        },
      ],
      visible: false,
    },
    {
      label: 'Diễn đàn',
      items: [
        {
          id: policies.Forum.View,
          label: 'Danh sách diễn đàn',
          icon: 'pi pi-table',
          routerLink: '/management/forum',
          visible: false,
        },
        {
          id: policies.Forum.Create,
          label: 'Tạo diễn đàn',
          icon: 'pi pi-pencil',
          routerLink: '/management/forum/create',
          visible: false,
        },
        {
          id: policies.Topic.Create,
          label: 'Tạo chủ đề',
          icon: 'pi pi-pencil',
          routerLink: '/management/forum/topic/create',
          visible: false,
        },
      ],
      visible: false,
    },
    {
      label: 'Prefix',
      items: [
        {
          id: policies.Prefix.View,
          label: 'Danh sách prefix',
          icon: 'pi pi-table',
          routerLink: '/management/prefix',
          visible: false,
        },
        {
          id: policies.Prefix.Create,
          label: 'Tạo prefix',
          icon: 'pi pi-pencil',
          routerLink: '/management/prefix/create',
          visible: false,
        },
      ],
      visible: false,
    },
    {
      label: 'Thành viên',
      items: [
        {
          id: policies.User.List,
          label: 'Danh sách thành viên',
          icon: 'pi pi-users',
          routerLink: '/management/user',
          visible: false,
        },
      ],
      visible: false,
    },
    {
      label: 'Vai trò',
      items: [
        {
          id: policies.Role.View,
          label: 'Danh sách vai trò',
          icon: 'pi pi-table',
          routerLink: '/management/role',
          visible: false,
        },
        {
          id: policies.Role.Create,
          label: 'Tạo vai trò',
          icon: 'pi pi-pencil',
          routerLink: '/management/role/create',
          visible: false,
        },
        {
          id: policies.Role.Assign,
          label: 'Gán vai trò',
          icon: 'pi pi-pencil',
          routerLink: '/management/role/assign',
          visible: false,
        },
      ],
      visible: false,
    },
  ];

  constructor(private userService: UserService) {
    super();
  }

  ngOnInit() {
    this.userService.permissions$
      .pipe(takeUntil(this.destroy$))
      .subscribe((permissions) => {
        if (!permissions.length) return;
        this.items = this.items.map((item) => ({
          ...item,
          visible: true,
          items: item.items?.map((subItem) => ({
            ...subItem,
            visible:
              permissions.includes('All') ||
              permissions.includes(subItem.id!) ||
              !subItem.id,
          })),
        }));

        this.items = this.items.map((item) => ({
          ...item,
          visible: item.items?.some((subItem) => subItem.visible),
        }));
      });
  }
}
