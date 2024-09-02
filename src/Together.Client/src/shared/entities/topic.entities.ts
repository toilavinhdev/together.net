export interface ITopicViewModel {
  id: string;
  subId: string;
  forumId: string;
  forumName: string;
  name: string;
  description?: string;
  postCount: number;
  replyCount: number;
}

export interface IGetTopicResponse {
  id: string;
  subId: string;
  forumId: string;
  name: string;
  description?: string;
}

export interface ICreateTopicRequest {
  forumId: string;
  name: string;
  description?: string;
}

export interface IUpdateTopicRequest {
  id: string;
  forumId: string;
  name: string;
  description?: string;
}
