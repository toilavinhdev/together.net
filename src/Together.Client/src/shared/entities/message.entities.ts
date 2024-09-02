import { IPaginationRequest, IPaginationResult } from '@/core/models';

export interface IListMessageRequest extends IPaginationRequest {
  conversationId: string;
}

export interface IListMessageResponse
  extends IPaginationResult<IMessageViewModel> {
  extra: { [key: string]: any };
}

export interface IMessageViewModel {
  id: string;
  subId: number;
  conversationId: string;
  text: string;
  createdAt: string;
  createdById: string;
  createdByUserName: string;
  createdByAvatar?: string;
}

export interface ISendMessageRequest {
  conversationId: string;
  text: string;
}

export interface ISendMessageResponse {
  id: string;
  subId: number;
  conversationId: string;
  text: string;
  createdAt: string;
}
