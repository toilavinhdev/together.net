import { ITopicViewModel } from '@/shared/entities/topic.entities';

export interface IForumViewModel {
  id: string;
  subId: number;
  name: string;
  topics?: ITopicViewModel[];
}

export interface ICreateForumRequest {
  name: string;
}

export interface IUpdateForumRequest {
  id: string;
  name: string;
}

export interface IGetForumResponse {
  id: string;
  subId: number;
  name: string;
  createdAt: string;
  modifiedAt?: string;
}
