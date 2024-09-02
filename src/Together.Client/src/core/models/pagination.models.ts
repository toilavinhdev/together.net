export interface IPaginationRequest {
  pageIndex: number;
  pageSize: number;
}

export interface IPagination {
  pageIndex: number;
  pageSize: number;
  totalRecord: number;
  totalPage: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface IPaginationResult<T> {
  pagination: IPagination;
  result: T[];
}
