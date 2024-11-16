import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ICloudinaryUploadImageResponse } from '@/shared/models/cloudinary.models';

@Injectable({
  providedIn: 'root',
})
export class CloudinaryService {
  private _cloudName = 'daraghioa';
  private _apiKey = '235213944124741';
  private _apiSecret = 'oWiXdGYq6rp0HEfkH_lobPjIsAQ';
  private _uploadPreset = 'togethernet';

  constructor(private client: HttpClient) {}

  uploadImage(file: File): Observable<ICloudinaryUploadImageResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('upload_preset', this._uploadPreset);
    formData.append('publicId', Date.now().toString());
    formData.append('apiKey', this._apiKey);
    const api = `https://api.cloudinary.com/v1_1/${this._cloudName}/image/upload`;
    return this.client.post<ICloudinaryUploadImageResponse>(api, formData);
  }
}
