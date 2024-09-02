import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import { BehaviorSubject, map, Observable } from 'rxjs';
import {
  ICreatePrefixRequest,
  IPrefixViewModel,
  IUpdatePrefixRequest,
} from '@/shared/entities/prefix.entities';
import { IBaseResponse } from '@/core/models';

@Injectable({
  providedIn: 'root',
})
export class PrefixService extends BaseService {
  prefixes$ = new BehaviorSubject<IPrefixViewModel[]>([]);

  constructor() {
    super();
    this.setEndpoint('community', 'prefix');
  }

  getPrefix(prefixId: string): Observable<IPrefixViewModel> {
    const url = this.createUrl(prefixId);
    return this.client
      .get<IBaseResponse<IPrefixViewModel>>(url)
      .pipe(map((response) => response.data));
  }

  listPrefix(): Observable<IPrefixViewModel[]> {
    const url = this.createUrl('/list');
    return this.client
      .get<IBaseResponse<IPrefixViewModel[]>>(url)
      .pipe(map((response) => response.data));
  }

  createPrefix(payload: ICreatePrefixRequest) {
    const url = this.createUrl('/create');
    return this.client.post(url, payload);
  }

  updatePrefix(payload: IUpdatePrefixRequest) {
    const url = this.createUrl('/update');
    return this.client.put(url, payload);
  }

  deletePrefix(prefixId: string) {
    const url = this.createUrl(prefixId);
    return this.client.delete(url);
  }
}
