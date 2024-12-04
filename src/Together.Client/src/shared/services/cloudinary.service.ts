import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ICloudinaryUploadImageResponse } from '@/shared/models/cloudinary.models';
import {
  cloudinaryUploadImageApiUrl,
  cloudinaryUploadImageForm,
} from '@/shared/utilities';

@Injectable({
  providedIn: 'root',
})
export class CloudinaryService {
  constructor(private client: HttpClient) {}

  uploadImage(file: File): Observable<ICloudinaryUploadImageResponse> {
    return this.client.post<ICloudinaryUploadImageResponse>(
      cloudinaryUploadImageApiUrl(),
      cloudinaryUploadImageForm(file),
    );
  }
}
