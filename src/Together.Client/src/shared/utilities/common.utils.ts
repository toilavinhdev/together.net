import { HttpErrorResponse } from '@angular/common/http';
import { IBaseResponse } from '@/core/models';

export function generateId() {
  return 'together-' + Date.now();
}

export function getErrorCode(error: Error): string {
  if (error instanceof HttpErrorResponse) {
    const response = error.error as IBaseResponse;
    return response?.errors?.[0].code ?? '';
  }
  return error.message;
}

export function getErrorMessage(error: Error): string {
  if (error instanceof HttpErrorResponse) {
    const response = error.error as IBaseResponse;
    return response?.errors?.[0].message ?? error.message;
  }
  return error.message;
}

export function isGUID(value: string) {
  const pattern =
    /^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$/;
  return pattern.test(value);
}

export function windowScrollToTop(behavior: ScrollBehavior = 'auto') {
  window.scrollTo({
    top: 0,
    behavior: behavior,
  });
}

export function scrollToTop(
  containerId: string,
  behavior: ScrollBehavior = 'auto',
) {
  const container = document.getElementById(containerId);
  if (!container) return;
  container.scrollTo({
    top: 0,
    behavior: behavior,
  });
}

export function scrollToBottom(
  containerId: string,
  behavior: ScrollBehavior = 'instant',
) {
  const container = document.getElementById(containerId);
  if (!container) return;
  container.scrollTo({
    top: container.scrollHeight,
    behavior: behavior,
  });
}

export function areArraysEqual<T>(arr1: T[], arr2: T[]) {
  if (arr1.length !== arr2.length) return false;
  const set = new Set(arr1);
  return arr2.every((item) => set.has(item));
}
