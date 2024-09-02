export interface IPrefixViewModel {
  id: string;
  subId: number;
  name: string;
  background: string;
  foreground: string;
  createdAt: string;
  modifiedAt?: string;
}

export interface ICreatePrefixRequest {
  name: string;
  foreground: string;
  background: string;
}

export interface IUpdatePrefixRequest {
  id: string;
  name: string;
  foreground: string;
  background: string;
}
