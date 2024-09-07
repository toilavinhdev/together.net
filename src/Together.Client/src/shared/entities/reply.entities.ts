import { IPaginationRequest, IPaginationResult } from '@/core/models';
import { EVoteType } from '@/shared/enums';
import { IVotePostRequest } from '@/shared/entities/post.entities';

export interface ICreateReplyRequest {
  postId: string;
  parentId?: string;
  body: string;
}

export interface ICreateReplyResponse {
  id: string;
  subId: number;
  postId: string;
  parentId?: string;
  level: number;
  body: string;
  createdAt: string;
}

export interface IUpdateReplyRequest {
  id: string;
  body: string;
}

export interface IListReplyRequest extends IPaginationRequest {
  postId?: string;
  parentId?: string;
  focusChildId?: string;
}

export interface IListReplyResponse
  extends IPaginationResult<IReplyViewModel> {}

export interface IReplyViewModel {
  id: string;
  subId: number;
  postId: string;
  parentId?: string;
  level: number;
  body: string;
  createdAt: string;
  createdById: string;
  createdByUserName: string;
  createdByAvatar?: string;
  voteUpCount: number;
  voteDownCount: number;
  voted?: EVoteType;
  childCount: number;
}

export interface IVoteReplyRequest {
  replyId: string;
  type: EVoteType;
}
