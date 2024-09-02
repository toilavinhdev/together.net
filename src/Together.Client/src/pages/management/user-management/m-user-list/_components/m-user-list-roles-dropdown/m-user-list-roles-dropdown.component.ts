import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { RoleService } from '@/shared/services';
import {
  IListRoleRequest,
  IRoleViewModel,
} from '@/shared/entities/role.entities';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'together-m-user-list-roles-dropdown',
  standalone: true,
  imports: [DropdownModule, FormsModule],
  templateUrl: './m-user-list-roles-dropdown.component.html',
})
export class MUserListRolesDropdownComponent
  extends BaseComponent
  implements OnInit
{
  @Output() roleIdChange = new EventEmitter<string>();

  roleId = '';

  roles: IRoleViewModel[] = [];

  params: IListRoleRequest = {
    pageIndex: 1,
    pageSize: 8,
  };

  loading = false;

  hasNextPage = true;

  constructor(private roleService: RoleService) {
    super();
  }

  ngOnInit() {}

  loadRoles() {
    if (!this.hasNextPage) return;
    this.loading = true;
    this.roleService
      .listRole(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.loading = false;
          this.roles =
            pagination.pageIndex === 1 ? result : [...this.roles, ...result];
          this.hasNextPage = pagination.hasNextPage;
        },
        error: (err) => {
          this.loading = false;
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  listenScrollReachedBottom() {
    const container = document.querySelector(
      '#m-user-list-roles-dropdown .p-dropdown-items-wrapper',
    );
    if (!container) return;
    container.addEventListener('scroll', () => {
      const reachedBottom =
        container.scrollTop + container.clientHeight >= container.scrollHeight;
      if (reachedBottom) {
        this.params.pageIndex++;
        this.loadRoles();
      }
    });
  }
}
