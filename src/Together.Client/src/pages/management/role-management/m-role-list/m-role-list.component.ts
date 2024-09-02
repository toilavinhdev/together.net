import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  TableCellDirective,
  TableColumnDirective,
  TableComponent,
} from '@/shared/components/elements';
import {
  IListRoleRequest,
  IRoleViewModel,
} from '@/shared/entities/role.entities';
import { RoleService, UserService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { Button } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { policies } from '@/shared/constants';
import { AsyncPipe, NgIf } from '@angular/common';

@Component({
  selector: 'together-m-role-list',
  standalone: true,
  imports: [
    TableComponent,
    TableColumnDirective,
    Button,
    TableCellDirective,
    RouterLink,
    ConfirmDialogModule,
    AsyncPipe,
    NgIf,
  ],
  templateUrl: './m-role-list.component.html',
  providers: [ConfirmationService],
})
export class MRoleListComponent extends BaseComponent implements OnInit {
  roles: IRoleViewModel[] = [];

  params: IListRoleRequest = {
    pageIndex: 1,
    pageSize: 12,
    userId: undefined,
  };

  constructor(
    private roleService: RoleService,
    private confirmationService: ConfirmationService,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.commonService.title$.next('Danh sách vai trò');
    this.loadRoles();
  }

  loadRoles() {
    this.roleService
      .listRole(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.roles = result;
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  confirmDelete(event: Event, roleId: string, roleName: string) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: `Dữ liệu của vai trò '${roleName}' khi xóa sẽ không thể hoàn tác`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.roleService
          .deleteRole(roleId)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.commonService.toast$.next({
                type: 'success',
                message: 'Xóa thành công',
              });
              this.roles = this.roles.filter((r) => r.id !== roleId);
            },
            error: (err) => {
              this.commonService.toast$.next({
                type: 'error',
                message: getErrorMessage(err),
              });
            },
          });
      },
      reject: () => {},
    });
  }

  protected readonly policies = policies;
}
