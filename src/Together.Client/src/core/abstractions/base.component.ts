import { Directive, inject, Input, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { CommonService } from '@/shared/services';

@Directive()
export class BaseComponent implements OnDestroy {
  @Input()
  componentClass = '';

  protected destroy$ = new Subject<void>();

  protected commonService = inject(CommonService);

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.destroy$.unsubscribe();
  }
}
