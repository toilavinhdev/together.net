import { Component, OnInit } from '@angular/core';
import {
  FormsModule,
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { BaseComponent } from '@/core/abstractions';
import { ForumService, TopicService, UserService } from '@/shared/services';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { takeUntil } from 'rxjs';
import { ContainerComponent } from '@/shared/components/elements';
import { Button } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { AsyncPipe, JsonPipe, NgIf } from '@angular/common';
import { IForumViewModel } from '@/shared/entities/forum.entities';
import { DropdownModule } from 'primeng/dropdown';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-m-topic-detail',
  standalone: true,
  imports: [
    ContainerComponent,
    Button,
    FormsModule,
    InputTextModule,
    NgIf,
    ReactiveFormsModule,
    RouterLink,
    DropdownModule,
    JsonPipe,
    AsyncPipe,
  ],
  templateUrl: './m-topic-detail.component.html',
})
export class MTopicDetailComponent extends BaseComponent implements OnInit {
  formType: 'create' | 'update' = 'create';

  form!: UntypedFormGroup;

  topicId = '';

  forums: IForumViewModel[] = [];

  constructor(
    private topicService: TopicService,
    private forumService: ForumService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private formBuilder: UntypedFormBuilder,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.loadForums();
    this.buildForm();
    this.getFormType();
  }

  private loadForums() {
    this.forumService
      .listForum()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.forums = data;
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  private getFormType() {
    this.activatedRoute.url.pipe(takeUntil(this.destroy$)).subscribe((url) => {
      if (url?.[1].path === 'create') {
        this.formType = 'create';
        this.commonService.title$.next('Tạo chủ đề');
      } else {
        this.formType = 'update';
        this.commonService.title$.next('Cập nhật chủ đề');
        this.topicId = url?.[1].path;
        this.patchValueForm();
      }
    });
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      id: [null],
      forumId: [null, [Validators.required]],
      name: [null, [Validators.required]],
      description: [null],
    });
  }

  private patchValueForm() {
    this.topicService
      .getTopic(this.topicId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.form.patchValue({ ...data });
        },
        error: (err) => {
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
    if (this.formType === 'create') {
      this.topicService
        .createTopic(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Tạo chủ đề thành công',
            });
            this.router.navigate(['/management/forum']).then();
          },
          error: (err) => {
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    } else {
      this.topicService
        .updateTopic(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Cập nhật chủ đề thành công',
            });
            this.router.navigate(['/management/forum']).then();
          },
          error: (err) => {
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
