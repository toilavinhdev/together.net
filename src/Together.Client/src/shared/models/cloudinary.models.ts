export interface ICloudinaryUploadImageResponse {
  asset_id: string;
  public_id: string;
  version: number;
  version_id: string;
  signature: string;
  width: number;
  height: number;
  format: string;
  resource_type: string;
  created_at: string;
  tags: string[];
  bytes: number;
  type: string;
  etag: string;
  placeholder: string;
  url: string;
  secure_url: string;
  asset_folder: string;
  display_name: string;
  access_mode: string;
  original_filename: string;
}
