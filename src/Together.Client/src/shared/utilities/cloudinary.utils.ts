import { ICloudinaryUploadImageResponse } from '@/shared/models/cloudinary.models';
import { environment } from '@/environments/environment';

export function cloudinaryUploadImageApiUrl() {
  return `https://api.cloudinary.com/v1_1/${environment.cloudinary.cloudName}/image/upload`;
}

export function cloudinaryUploadImageForm(file: File): FormData {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('upload_preset', environment.cloudinary.uploadPreset);
  formData.append('publicId', Date.now().toString());
  formData.append('apiKey', environment.cloudinary.apiKey);
  return formData;
}

export function cloudinaryUploadImageXHR(
  file: File,
  callBackSuccess: (response: ICloudinaryUploadImageResponse) => void,
  callBackFailed?: () => void,
) {
  const xhr = new XMLHttpRequest();
  xhr.open('POST', cloudinaryUploadImageApiUrl(), true);
  xhr.onload = () => {
    if (xhr.status === 200) {
      const response = JSON.parse(xhr.responseText);
      callBackSuccess(response);
    } else {
      callBackFailed?.();
    }
  };
  xhr.send(cloudinaryUploadImageForm(file));
}
