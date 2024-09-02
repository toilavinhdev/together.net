import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import {
  IAssignRoleRequest,
  ICreateRoleRequest,
  IGetRoleResponse,
  IListRoleRequest,
  IListRoleResponse,
  IUpdateRoleRequest,
} from '@/shared/entities/role.entities';
import { map, Observable } from 'rxjs';
import { IBaseResponse } from '@/core/models';

@Injectable({
  providedIn: 'root',
})
export class RoleService extends BaseService {
  constructor() {
    super();
    this.setEndpoint('identity', 'role');
  }

  getRole(roleId: string): Observable<IGetRoleResponse> {
    const url = this.createUrl(roleId);
    return this.client
      .get<IBaseResponse<IGetRoleResponse>>(url)
      .pipe(map((response) => response.data));
  }

  listRole(params: IListRoleRequest): Observable<IListRoleResponse> {
    const url = this.createUrl('/list');
    return this.client
      .get<
        IBaseResponse<IListRoleResponse>
      >(url, { params: this.createParams(params) })
      .pipe(map((response) => response.data));
  }

  createRole(payload: ICreateRoleRequest) {
    const url = this.createUrl('/create');
    return this.client.post(url, payload);
  }

  updateRole(payload: IUpdateRoleRequest) {
    const url = this.createUrl('/update');
    return this.client.put(url, payload);
  }

  assignRole(payload: IAssignRoleRequest) {
    const url = this.createUrl('/assign');
    return this.client.post(url, payload);
  }

  deleteRole(roleId: string) {
    const url = this.createUrl(roleId);
    return this.client.delete(url);
  }
}
