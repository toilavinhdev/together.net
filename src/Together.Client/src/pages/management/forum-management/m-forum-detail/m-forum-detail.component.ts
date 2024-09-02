import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntil } from 'rxjs';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ContainerComponent } from '@/shared/components/elements';
import { AsyncPipe, NgIf } from '@angular/common';
import { Button } from 'primeng/button';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { ForumService, UserService } from '@/shared/services';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-m-forum-detail',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    InputTextModule,
    ContainerComponent,
    NgIf,
    Button,
    RouterLink,
    AsyncPipe,
  ],
  templateUrl: './m-forum-detail.component.html',
})
export class MForumDetailComponent extends BaseComponent implements OnInit {
  formType: 'create' | 'update' = 'create';

  form!: UntypedFormGroup;

  forumId = '';

  loadingGet = false;

  loadingSubmit = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private formBuilder: UntypedFormBuilder,
    private forumService: ForumService,
    private router: Router,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
    this.getFormType();
  }

  private getFormType() {
    this.activatedRoute.url.pipe(takeUntil(this.destroy$)).subscribe((url) => {
      if (url?.[0].path === 'create') {
        this.formType = 'create';
        this.commonService.title$.next('Tạo diễn đàn');
      } else {
        this.commonService.title$.next('Cập nhật diễn đàn');
        this.formType = 'update';
        this.forumId = url?.[0].path;
        this.patchFormValue();
      }
    });
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      id: [null],
      name: [null, [Validators.required]],
    });
  }

  private patchFormValue() {
    if (!this.forumId) return;
    this.loadingGet = true;
    this.forumService
      .getForum(this.forumId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.loadingGet = false;
          this.form.patchValue({ ...data });
        },
        error: (err) => {
          this.loadingGet = false;
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  onSubmit() {
    if (this.form.invalid) {
      markFormDirty(this.form);
      return;
    }
    this.loadingSubmit = true;
    if (this.formType === 'create') {
      this.forumService
        .createForum(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.loadingSubmit = false;
            this.commonService.toast$.next({
              type: 'success',
              message: 'Tạo diễn đàn thành công',
            });
            this.router.navigate(['/management/forum']).then();
          },
          error: (err) => {
            this.loadingSubmit = false;
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    } else {
      this.forumService
        .updateForum(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.loadingSubmit = false;
            this.commonService.toast$.next({
              type: 'success',
              message: 'Cập nhật diễn đàn thành công',
            });
            this.router.navigate(['/management/forum']).then();
          },
          error: (err) => {
            this.loadingSubmit = false;
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    }
  }

  protected readonly policies = policies;
}
