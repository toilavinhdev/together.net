import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import { map, Observable } from 'rxjs';
import {
  ICreateReplyRequest,
  ICreateReplyResponse,
  IListReplyRequest,
  IListReplyResponse,
  IUpdateReplyRequest,
  IVoteReplyRequest,
} from '@/shared/entities/reply.entities';
import { IBaseResponse } from '@/core/models';
import { IVoteResponse } from '@/shared/entities/post.entities';

@Injectable({
  providedIn: 'root',
})
export class ReplyService extends BaseService {
  constructor() {
    super();
    this.setEndpoint('community', 'reply');
  }

  listReply(params: IListReplyRequest): Observable<IListReplyResponse> {
    const url = this.createUrl('/list');
    return this.client
      .get<
        IBaseResponse<IListReplyResponse>
      >(url, { params: this.createParams(params) })
      .pipe(map((response) => response.data));
  }

  createReply(payload: ICreateReplyRequest): Observable<ICreateReplyResponse> {
    const url = this.createUrl('/create');
    return this.client
      .post<IBaseResponse<ICreateReplyResponse>>(url, payload)
      .pipe(map((response) => response.data));
  }

  voteReply(payload: IVoteReplyRequest): Observable<IVoteResponse> {
    const url = this.createUrl('/vote');
    return this.client
      .post<IBaseResponse<IVoteResponse>>(url, payload)
      .pipe(map((response) => response.data));
  }

  updateReply(payload: IUpdateReplyRequest) {
    const url = this.createUrl('/update');
    return this.client.put(url, payload);
  }

  deleteReply(id: string) {
    const url = this.createUrl(id);
    return this.client.delete<IBaseResponse<IVoteResponse>>(url);
  }
}
