import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import { BehaviorSubject, map, Observable } from 'rxjs';
import {
  ICreatePostRequest,
  IGetPostResponse,
  IListPostRequest,
  IListPostResponse,
  IPostViewModel,
  IUpdatePostRequest,
  IVotePostRequest,
  IVoteResponse,
} from '@/shared/entities/post.entities';
import { IBaseResponse } from '@/core/models';

@Injectable({
  providedIn: 'root',
})
export class PostService extends BaseService {
  posts$ = new BehaviorSubject<IPostViewModel[]>([]);

  constructor() {
    super();
    this.setEndpoint('community', 'post');
  }

  getPost(postId: string): Observable<IGetPostResponse> {
    const url = this.createUrl(postId);
    return this.client
      .get<IBaseResponse<IGetPostResponse>>(url)
      .pipe(map((response) => response.data));
  }

  listPost(params: IListPostRequest): Observable<IListPostResponse> {
    const url = this.createUrl('/list');
    return this.client
      .get<IBaseResponse<IListPostResponse>>(url, {
        params: this.createParams(params),
      })
      .pipe(map((response) => response.data));
  }

  createPost(payload: ICreatePostRequest) {
    const url = this.createUrl('/create');
    return this.client.post(url, payload);
  }

  votePost(payload: IVotePostRequest): Observable<IVoteResponse> {
    const url = this.createUrl('/vote');
    return this.client
      .post<IBaseResponse<IVoteResponse>>(url, payload)
      .pipe(map((response) => response.data));
  }

  updatePost(payload: IUpdatePostRequest) {
    const url = this.createUrl('/update');
    return this.client.put(url, payload);
  }

  deletePost(postId: string) {
    const url = this.createUrl(postId);
    return this.client.delete(url);
  }
}
