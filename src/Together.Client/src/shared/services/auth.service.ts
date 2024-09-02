import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import {
  ICurrentUserClaims,
  IExternalAuthRequest,
  IForgotPasswordRequest,
  IRefreshTokenResponse,
  ISignInRequest,
  ISignInResponse,
  ISignUpRequest,
  ISubmitForgotPasswordTokenRequest,
} from '@/shared/entities/auth.entities';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { IBaseResponse } from '@/core/models';
import { localStorageKeys } from '@/shared/constants';
import { jwtDecode } from 'jwt-decode';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class AuthService extends BaseService {
  refreshingToken$ = new BehaviorSubject<boolean>(false);

  accessToken$ = new BehaviorSubject<string | undefined>(undefined);

  constructor(private cookieService: CookieService) {
    super();
    this.setEndpoint('identity', 'auth');
  }

  signIn(payload: ISignInRequest): Observable<ISignInResponse> {
    const url = this.createUrl('/sign-in');
    return this.client
      .post<IBaseResponse<ISignInResponse>>(url, payload)
      .pipe(map((r) => r.data));
  }

  refreshToken(): Observable<IRefreshTokenResponse> {
    const url = this.createUrl('/refresh-token');
    return this.client
      .post<IBaseResponse<ISignInResponse>>(url, {
        accessToken: this.getAT(),
        refreshToken: this.getRT(),
      })
      .pipe(map((r) => r.data));
  }

  signUp(payload: ISignUpRequest) {
    const url = this.createUrl('/sign-up');
    return this.client.post(url, payload);
  }

  externalAuth(payload: IExternalAuthRequest): Observable<ISignInResponse> {
    const url = this.createUrl('/external');
    return this.client
      .post<IBaseResponse<ISignInResponse>>(url, payload)
      .pipe(map((response) => response.data));
  }

  logout() {
    const url = this.createUrl('/logout');
    return this.client.post(url, {});
  }

  forgotPassword(payload: IForgotPasswordRequest) {
    const url = this.createUrl('/forgot-password');
    return this.client.post(url, payload);
  }

  submitForgotPasswordToken(payload: ISubmitForgotPasswordTokenRequest) {
    const url = this.createUrl('/forgot-password/submit');
    return this.client.post(url, payload);
  }

  setToken(at: string, rt: string) {
    localStorage.setItem(localStorageKeys.ACCESS_TOKEN, at);
    localStorage.setItem(localStorageKeys.REFRESH_TOKEN, rt);
    localStorage.setItem(
      localStorageKeys.CURRENT_USER,
      JSON.stringify(jwtDecode<ICurrentUserClaims>(at)),
    );
  }

  removeToken() {
    localStorage.removeItem(localStorageKeys.ACCESS_TOKEN);
    localStorage.removeItem(localStorageKeys.REFRESH_TOKEN);
    localStorage.removeItem(localStorageKeys.CURRENT_USER);
  }

  getAT() {
    return localStorage.getItem(localStorageKeys.ACCESS_TOKEN);
  }

  getRT() {
    return localStorage.getItem(localStorageKeys.REFRESH_TOKEN);
  }

  getClaims = (): ICurrentUserClaims | undefined => {
    const json = localStorage.getItem(localStorageKeys.CURRENT_USER);
    if (!json) return undefined;
    return JSON.parse(json);
  };

  isATValid() {
    const claims = this.getClaims();
    if (!claims) return false;
    return claims.exp > Math.floor(Date.now() / 1000);
  }
}
