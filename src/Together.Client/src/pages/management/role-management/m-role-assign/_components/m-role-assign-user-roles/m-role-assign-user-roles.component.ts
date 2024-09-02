import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Button } from 'primeng/button';
import {
  TableCellDirective,
  TableColumnDirective,
  TableComponent,
} from '@/shared/components/elements';
import { BaseComponent } from '@/core/abstractions';
import { RoleService } from '@/shared/services';
import {
  IListRoleRequest,
  IRoleViewModel,
} from '@/shared/entities/role.entities';
import { takeUntil } from 'rxjs';
import { areArraysEqual, getErrorMessage } from '@/shared/utilities';
import { DragDropModule } from 'primeng/dragdrop';

@Component({
  selector: 'together-m-role-assign-user-roles',
  standalone: true,
  imports: [
    Button,
    TableCellDirective,
    TableComponent,
    TableColumnDirective,
    DragDropModule,
  ],
  templateUrl: './m-role-assign-user-roles.component.html',
})
export class MRoleAssignUserRolesComponent extends BaseComponent {
  @Output() userRolesChange = new EventEmitter<IRoleViewModel[]>();

  @Input() rowDraggableDroppable = '';

  draggingRole?: IRoleViewModel;

  roles: IRoleViewModel[] = [];

  private _originalRoles: IRoleViewModel[] = [];

  get rolesHasChanged(): boolean {
    return !areArraysEqual(this.roles, this._originalRoles);
  }

  params: IListRoleRequest = {
    pageIndex: 1,
    pageSize: 10,
    userId: undefined,
  };

  loading = false;

  constructor(private roleService: RoleService) {
    super();
  }

  loadRoles(userId?: string) {
    if (this.params.userId !== userId) {
      this.roles = [];
      this._originalRoles = [];
    }
    this.params.userId = userId;
    this.loading = true;
    this.roleService
      .listRole(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.loading = false;
          this.roles = result;
          this._originalRoles = [...this.roles];
          this.userRolesChange.emit(this.roles);
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

  removeRole(role: IRoleViewModel) {
    this.roles = this.roles.filter((userRole) => userRole.id !== role.id);
    this.userRolesChange.emit(this.roles);
  }

  onCancel() {
    this.roles = [...this._originalRoles];
    this.userRolesChange.emit(this.roles);
  }

  onSubmit() {
    if (!this.params.userId) return;
    if (!this.roles.length) {
      this.commonService.toast$.next({
        type: 'warn',
        message: 'Thành viên phải có ít nhất 1 vai trò',
      });
      return;
    }
    this.roleService
      .assignRole({
        userId: this.params.userId,
        roleIds: this.roles.map((role) => role.id),
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.commonService.toast$.next({
            type: 'success',
            message: 'Gán vai trò thành công',
          });
          this._originalRoles = [...this.roles];
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  onDrop() {
    console.log('drop', this.draggingRole);
    if (!this.draggingRole) return;
    if (!this.params.userId) {
      this.commonService.toast$.next({
        type: 'error',
        message: 'Vui lòng chọn thành viên',
      });
      return;
    }
    this.roles = [...this.roles, this.draggingRole];
    this.userRolesChange.emit(this.roles);
  }
}
