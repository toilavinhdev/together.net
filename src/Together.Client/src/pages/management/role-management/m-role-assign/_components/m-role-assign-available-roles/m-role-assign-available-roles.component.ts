import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  TableColumnDirective,
  TableComponent,
} from '@/shared/components/elements';
import {
  IListRoleRequest,
  IRoleViewModel,
} from '@/shared/entities/role.entities';
import { BaseComponent } from '@/core/abstractions';
import { RoleService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';

@Component({
  selector: 'together-m-role-assign-available-roles',
  standalone: true,
  imports: [TableComponent, TableColumnDirective],
  templateUrl: './m-role-assign-available-roles.component.html',
})
export class MRoleAssignAvailableRolesComponent
  extends BaseComponent
  implements OnInit
{
  @Output() rowDraggingChange = new EventEmitter<IRoleViewModel>();

  @Input() rowDraggableDroppable = '';

  private _roles: IRoleViewModel[] = [];

  existedRoles: IRoleViewModel[] = [];

  get availableRoles(): IRoleViewModel[] {
    return this._roles.filter(
      (availableRole) =>
        !this.existedRoles.some((userRole) => userRole.id === availableRole.id),
    );
  }

  params: IListRoleRequest = {
    pageIndex: 1,
    pageSize: 10,
  };

  loading = false;

  private _draggingRole?: IRoleViewModel;

  constructor(private roleService: RoleService) {
    super();
  }

  ngOnInit() {
    this.loadRoles();
  }

  loadRoles() {
    this.roleService
      .listRole(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result }) => {
          this._roles = result;
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  onDragStart(role: IRoleViewModel) {
    console.log(role);
    this._draggingRole = role;
    this.rowDraggingChange.emit(this._draggingRole);
  }

  onDragEnd() {
    this._roles = this._roles.filter(
      (role) => role.id !== this._draggingRole?.id,
    );
    this._draggingRole = undefined;
  }
}
