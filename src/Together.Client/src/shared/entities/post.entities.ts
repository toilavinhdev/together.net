import {
  IGeneralUser,
  IPaginationRequest,
  IPaginationResult,
} from '@/core/models';
import { EVoteType } from '@/shared/enums';

export interface IListPostRequest extends IPaginationRequest {
  topicId?: string;
  userId?: string;
  search?: string;
}

export interface IListPostResponse extends IPaginationResult<IPostViewModel> {
  extra: { [key: string]: string };
}

export interface IPostViewModel {
  id: string;
  subId: number;
  forumId: string;
  topicId: string;
  topicName: string;
  prefixId?: string;
  prefixName?: string;
  prefixForeground?: string;
  prefixBackground?: string;
  title: string;
  body: string;
  createdAt: string;
  author: IGeneralUser;
  replyCount: number;
  postCount: number;
}

export interface ICreatePostRequest {
  topicId: string;
  prefixId?: string;
  title: string;
  body: string;
}

export interface IUpdatePostRequest {
  id: string;
  topicId: string;
  prefixId?: string;
  title: string;
  body: string;
}

export interface IGetPostResponse {
  id: string;
  subId: string;
  forumId: string;
  forumName: string;
  topicId: string;
  topicName: string;
  prefixName?: string;
  prefixForeground?: string;
  prefixBackground?: string;
  title: string;
  body: string;
  createdAt: string;
  createdById: string;
  createdByUserName: string;
  createdByAvatar?: string;
  replyCount: number;
  voteUpCount: number;
  voteDownCount: number;
  voted?: EVoteType;
  viewCount: number;
}

export interface IVotePostRequest {
  postId: string;
  type: EVoteType;
}

export interface IVoteResponse {
  sourceId: string;
  value?: EVoteType;
  isVoted: boolean;
}
