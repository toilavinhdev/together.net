import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ContainerComponent } from '@/shared/components/elements';
import {
  MRoleAssignAvailableRolesComponent,
  MRoleAssignUserRolesComponent,
  MRoleAssignUsersComponent,
} from '@/pages/management/role-management/m-role-assign/_components';

@Component({
  selector: 'together-m-role-assign',
  standalone: true,
  imports: [
    ContainerComponent,
    MRoleAssignUsersComponent,
    MRoleAssignUserRolesComponent,
    MRoleAssignAvailableRolesComponent,
  ],
  templateUrl: './m-role-assign.component.html',
})
export class MRoleAssignComponent extends BaseComponent implements OnInit {
  readonly rowDraggableDroppable = 'draggingRole';

  ngOnInit() {
    this.commonService.title$.next('Gán vai trò');
  }
}
