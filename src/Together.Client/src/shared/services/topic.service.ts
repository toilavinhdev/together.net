import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import { map, Observable } from 'rxjs';
import {
  ICreateTopicRequest,
  IGetTopicResponse,
  IUpdateTopicRequest,
} from '@/shared/entities/topic.entities';
import { IBaseResponse } from '@/core/models';

@Injectable({
  providedIn: 'root',
})
export class TopicService extends BaseService {
  constructor() {
    super();
    this.setEndpoint('community', 'topic');
  }

  getTopic(topicId: string): Observable<IGetTopicResponse> {
    const url = this.createUrl(topicId);
    return this.client
      .get<IBaseResponse<IGetTopicResponse>>(url)
      .pipe(map((response) => response.data));
  }

  createTopic(payload: ICreateTopicRequest) {
    const url = this.createUrl('/create');
    return this.client.post(url, payload);
  }

  updateTopic(payload: IUpdateTopicRequest) {
    const url = this.createUrl('/update');
    return this.client.put(url, payload);
  }

  deleteTopic(topicId: string) {
    const url = this.createUrl(topicId);
    return this.client.delete(url);
  }
}
