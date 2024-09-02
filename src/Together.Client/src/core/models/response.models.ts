export interface IBaseResponse<T = any> {
  success: boolean;
  statusCode: number;
  errors?: IBaseError[];
  data: T;
}

export interface IBaseError {
  code: string;
  message: string;
  parameter?: string;
}
