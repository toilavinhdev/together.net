import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import { IUploadFileRequest } from '@/shared/entities/file.entities';
import { map, Observable } from 'rxjs';
import { IBaseResponse } from '@/core/models';

@Injectable({
  providedIn: 'root',
})
export class FileService extends BaseService {
  constructor() {
    super();
    this.setEndpoint('storage', 'file');
  }

  uploadFile(
    request: IUploadFileRequest,
  ): Observable<{ url: string; publicId: string }> {
    let formData = new FormData();
    formData.append('file', request.file);
    if (request.bucket) {
      formData.append('bucket', request.bucket);
    }
    const url = this.createUrl('/upload');
    return this.client
      .post<IBaseResponse<{ url: string; publicId: string }>>(url, formData)
      .pipe(map((response) => response.data));
  }

  deleteFile(publicId: string) {
    const url = this.createUrl('/delete');
    return this.client.post(url, { publicId });
  }
}
