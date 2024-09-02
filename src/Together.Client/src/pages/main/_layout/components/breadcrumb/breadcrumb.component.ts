import { Component, OnInit } from '@angular/core';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { BaseComponent } from '@/core/abstractions';
import { MenuItem } from 'primeng/api';
import { takeUntil } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';
import { AsyncPipe, NgIf } from '@angular/common';

@Component({
  selector: 'together-breadcrumb',
  standalone: true,
  imports: [BreadcrumbModule, TranslateModule, NgIf, AsyncPipe],
  templateUrl: './breadcrumb.component.html',
})
export class BreadcrumbComponent extends BaseComponent implements OnInit {
  home: MenuItem = {
    routerLink: '/',
    icon: 'pi pi-home',
  };

  items: MenuItem[] = [];

  ngOnInit() {
    this.commonService.breadcrumb$
      .pipe(takeUntil(this.destroy$))
      .subscribe((items) => {
        this.items = items.map(
          (item) =>
            ({
              label: item.title,
              routerLink: item.routerLink,
            }) as MenuItem,
        );
      });
  }
}
