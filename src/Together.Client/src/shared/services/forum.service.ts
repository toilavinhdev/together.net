import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import {
  ICreateForumRequest,
  IForumViewModel,
  IGetForumResponse,
  IUpdateForumRequest,
} from '@/shared/entities/forum.entities';
import { IBaseResponse } from '@/core/models';
import { BaseService } from '@/core/abstractions';

@Injectable({
  providedIn: 'root',
})
export class ForumService extends BaseService {
  forums$ = new BehaviorSubject<IForumViewModel[]>([]);

  constructor() {
    super();
    this.setEndpoint('community', 'forum');
  }

  getForum(id: string): Observable<IGetForumResponse> {
    const url = this.createUrl(id);
    return this.client
      .get<IBaseResponse<IGetForumResponse>>(url)
      .pipe(map((response) => response.data));
  }

  listForum(includeTopics: boolean = false): Observable<IForumViewModel[]> {
    const url = this.createUrl('/list');
    return this.client
      .get<IBaseResponse<IForumViewModel[]>>(url, {
        params: {
          includeTopics,
        },
      })
      .pipe(map((response) => response.data));
  }

  createForum(payload: ICreateForumRequest) {
    const url = this.createUrl('/create');
    return this.client.post(url, payload);
  }

  updateForum(payload: IUpdateForumRequest) {
    const url = this.createUrl('/update');
    return this.client.put(url, payload);
  }

  deleteForum(forumId: string) {
    const url = this.createUrl(forumId);
    return this.client.delete(url);
  }
}
