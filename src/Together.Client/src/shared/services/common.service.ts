import { Injectable } from '@angular/core';
import { Params, Router } from '@angular/router';
import { BehaviorSubject, Subject } from 'rxjs';
import { IToast } from '@/shared/models/toast.models';
import { IBreadcrumbItem } from '@/shared/models/breadcrumb.models';
import { localStorageKeys } from '@/shared/constants';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '@/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CommonService {
  title$ = new BehaviorSubject<string>('');

  spinning$ = new Subject<boolean>();

  toast$ = new BehaviorSubject<IToast | undefined>(undefined);

  breadcrumb$ = new BehaviorSubject<IBreadcrumbItem[]>([]);

  constructor(
    private router: Router,
    private translateService: TranslateService,
  ) {}

  navigateToLogin() {
    this.router.navigate(['/', 'auth', 'sign-in']).then();
  }

  navigateToMain() {
    this.router.navigate(['/']).then();
  }

  navigateToAdminPage() {
    this.router.navigate(['/management']).then();
  }

  navigateToTopic(topicId: string) {
    this.router.navigate(['/', 'topics', topicId]).then();
  }

  navigateToCreatePost(topicId?: string) {
    this.router.navigate(['/', 'topics', topicId ?? 0, 'create-post']).then();
  }

  navigateToPost(postId: string, queryParams?: Params) {
    this.router
      .navigate(['/', 'posts', postId], {
        queryParamsHandling: 'merge',
        queryParams: queryParams,
      })
      .then();
  }

  navigateToUpdatePost(postId: string) {
    this.router.navigate(['/', 'posts', postId, 'update-post']).then();
  }

  navigateToProfile(userId: string) {
    this.router.navigate(['/', 'user', userId]).then();
  }

  navigateToConversation(conversationId: string) {
    this.router.navigate(['/', 'conversations', conversationId]).then();
  }

  translate(key: string) {
    return this.translateService.instant(key);
  }

  getCurrentLanguage() {
    return localStorage.getItem(localStorageKeys.LANG) || environment.lang;
  }

  setLanguage(lang: string) {
    localStorage.setItem(localStorageKeys.LANG, lang);
    this.translateService.use(lang);
  }
}
