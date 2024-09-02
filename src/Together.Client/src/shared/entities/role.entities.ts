import { IPaginationRequest, IPaginationResult } from '@/core/models';

export interface IListRoleRequest extends IPaginationRequest {
  userId?: string;
}

export interface IListRoleResponse extends IPaginationResult<IRoleViewModel> {}

export interface IRoleViewModel {
  id: string;
  subId: number;
  name: string;
  description?: string;
  createdAt: string;
  modifiedAt?: string;
}

export interface IGetRoleResponse {
  id: string;
  subId: number;
  name: string;
  description?: string;
  claims: string[];
  createdAt: string;
  modifiedAt?: string;
}

export interface ICreateRoleRequest {
  name: string;
  description?: string;
  claims: string[];
}

export interface IUpdateRoleRequest extends ICreateRoleRequest {
  id: string;
}

export interface IAssignRoleRequest {
  userId: string;
  roleIds: string[];
}
