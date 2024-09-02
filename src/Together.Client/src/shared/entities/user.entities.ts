import { EGender, EUserStatus } from '@/shared/enums';
import { IPaginationRequest, IPaginationResult } from '@/core/models';

export interface IMeResponse {
  id: string;
  subId: number;
  userName: string;
  email: string;
  status: EUserStatus;
  avatar?: string;
}

export interface IGetUserResponse {
  id: string;
  subId: number;
  createdAt: string;
  userName: string;
  gender: EGender;
  fullName?: string;
  avatar?: string;
  biography?: string;
  postCount: number;
  replyCount: number;
}

export interface IUpdateProfileRequest {
  userName: string;
  gender: EGender;
  fullName?: string;
  biography?: string;
}

export interface IUpdatePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface IListUserRequest extends IPaginationRequest {
  search?: string;
  roleId?: string;
}

export interface IListUserResponse extends IPaginationResult<IUserViewModel> {}

export interface IUserViewModel {
  id: string;
  subId: string;
  userName: string;
  status: EUserStatus;
  email: string;
  fullName?: string;
  avatar?: string;
  online: boolean;
}
