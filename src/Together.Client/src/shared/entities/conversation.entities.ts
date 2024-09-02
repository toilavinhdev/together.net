import { IPaginationRequest, IPaginationResult } from '@/core/models';
import { EConversationType } from '@/shared/enums';

export interface IListConversationRequest extends IPaginationRequest {}

export interface IListConversationResponse
  extends IPaginationResult<IConversationViewModel> {}

export interface IConversationViewModel {
  id: string;
  subId: number;
  name: string;
  image?: string;
  lastMessageByUserId?: string;
  lastMessageByUserName?: string;
  lastMessageText?: string;
  lastMessageAt?: string;
}

export interface IGetConversationRequest {
  conversationId?: string;
  privateReceiverId?: string;
}

export interface ICreateConversationRequest {
  otherParticipantIds: string[];
  type: EConversationType;
  name?: string;
}
