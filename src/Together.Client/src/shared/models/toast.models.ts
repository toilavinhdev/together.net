export interface IToast {
  type: 'success' | 'info' | 'warn' | 'error';
  message?: string;
}
