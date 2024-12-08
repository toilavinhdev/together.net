import { inject, Injectable } from '@angular/core';
import { environment } from '@/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable()
export abstract class BaseService {
  private host = this.normalizePath(environment.host);

  private apiVersion = 'v1';

  private serviceName = '';

  private endpoint = '';

  protected client = inject(HttpClient);

  protected setEndpoint(serviceName: string, endpoint: string) {
    this.serviceName = this.normalizePath(serviceName);
    this.endpoint = this.normalizePath(endpoint);
  }

  protected createUrl(path: string, apiVersion?: string) {
    return `${this.host}/api/${this.serviceName}/${apiVersion ?? this.apiVersion}/${this.endpoint}/${this.normalizePath(path)}`;
  }

  protected createParams(obj: object): HttpParams {
    return Object.entries(obj).reduce(
      (params, [key, value]) =>
        value !== undefined && value !== null ? params.set(key, value) : params,
      new HttpParams(),
    );
  }

  private normalizePath(value: string): string {
    if (value.startsWith('/')) return value.slice(1);
    if (value.endsWith('/')) return value.slice(0, -1);
    return value;
  }
}
