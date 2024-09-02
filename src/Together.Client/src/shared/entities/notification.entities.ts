import { IPaginationRequest, IPaginationResult } from '@/core/models';
import { IGeneralUser } from '@/core/models';
import { ENotificationStatus, ENotificationType } from '@/shared/enums';

export interface IListNotificationRequest extends IPaginationRequest {}

export interface IListNotificationResponse
  extends IPaginationResult<INotificationViewModel> {}

export interface INotificationViewModel {
  id: string;
  subId: number;
  subject: IGeneralUser;
  type: ENotificationType;
  directObjectId: string;
  indirectObjectId?: string;
  prepositionalObjectId?: string;
  status: ENotificationStatus;
  createdAt: string;
  modifiedAt: string;
}
