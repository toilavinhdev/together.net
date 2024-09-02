import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  FormsModule,
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { AsyncPipe, JsonPipe, NgIf } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ForumService, PostService, PrefixService } from '@/shared/services';
import { combineLatest, map, Observable, take, takeUntil } from 'rxjs';
import { Button } from 'primeng/button';
import { getErrorMessage, isGUID, markFormDirty } from '@/shared/utilities';
import { SelectItem, SelectItemGroup } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { PrefixComponent } from '@/shared/components/elements';
import { IPrefixViewModel } from '@/shared/entities/prefix.entities';
import { EditorComponent } from '@/shared/components/controls';

@Component({
  selector: 'together-post-edit',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    JsonPipe,
    InputTextModule,
    DropdownModule,
    AsyncPipe,
    Button,
    FormsModule,
    NgIf,
    PrefixComponent,
    EditorComponent,
  ],
  templateUrl: './post-edit.component.html',
})
export class PostEditComponent extends BaseComponent implements OnInit {
  @ViewChild('editor') editor!: EditorComponent;

  formType: 'create' | 'update' = 'create';

  postId = '';

  form!: UntypedFormGroup;

  forums$!: Observable<SelectItemGroup[]>;

  loading = false;

  selectedPrefix?: IPrefixViewModel;

  loadingPrefix = false;

  loadingTopic = false;

  constructor(
    private formBuilder: UntypedFormBuilder,
    protected prefixService: PrefixService,
    protected forumService: ForumService,
    private activatedRoute: ActivatedRoute,
    private postService: PostService,
    private cdk: ChangeDetectorRef,
  ) {
    super();
  }

  ngOnInit() {
    this.getFormType();
    this.buildForm();
    this.routerTracking();
    this.loadTopics();
    this.loadPrefixes();
    this.forums$ = this.forumService.forums$.pipe(
      map((data) =>
        data.map(
          (forum) =>
            ({
              label: forum.name,
              items: forum.topics?.map(
                (topic) =>
                  ({
                    label: topic.name,
                    value: topic.id,
                  }) as SelectItem,
              ),
            }) as SelectItemGroup,
        ),
      ),
    );
  }

  onSubmit() {
    if (this.form.invalid) {
      markFormDirty(this.form);
      return;
    }
    this.loading = true;
    if (this.formType === 'create') {
      this.postService
        .createPost(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.loading = false;
            this.commonService.toast$.next({
              type: 'success',
              message: 'Tạo bài viết thành công',
            });
            this.commonService.navigateToTopic(this.form.get('topicId')?.value);
          },
          error: (err) => {
            this.loading = false;
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    } else if (this.formType === 'update') {
      this.postService
        .updatePost(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.navigateToPost(this.postId);
            this.commonService.toast$.next({
              type: 'success',
              message: 'Cập nhật bài viết thành công',
            });
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

  private getFormType() {
    this.activatedRoute.url.pipe(takeUntil(this.destroy$)).subscribe((url) => {
      if (url?.[0].path === 'topics' && url?.[2].path === 'create-post') {
        this.formType = 'create';
        this.commonService.breadcrumb$.next([
          {
            title: 'Tạo bài viết',
          },
        ]);
      }
      if (url?.[0].path === 'posts' && url?.[2].path === 'update-post') {
        this.commonService.breadcrumb$.next([
          {
            title: 'Cập nhật bài viết',
          },
        ]);
        this.formType = 'update';
        this.postId = url?.[1].path;
        this.patchFormValue();
      }
    });
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      id: [null],
      topicId: [null, [Validators.required]],
      prefixId: [null],
      title: [null, [Validators.required]],
      body: [null, [Validators.required]],
    });
    combineLatest(
      this.form.get('prefixId')!.valueChanges as Observable<string>,
      this.prefixService.prefixes$.asObservable(),
    )
      .pipe(
        takeUntil(this.destroy$),
        map(([selectedPrefixId, prefixes]) =>
          prefixes.find((p) => p.id === selectedPrefixId),
        ),
      )
      .subscribe((selectedPrefix) => (this.selectedPrefix = selectedPrefix));
  }

  private loadPrefixes() {
    this.prefixService.prefixes$.pipe(take(1)).subscribe((exists) => {
      if (exists.length > 0) return;
      this.loadingPrefix = true;
      this.prefixService
        .listPrefix()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.loadingPrefix = false;
            this.prefixService.prefixes$.next(data);
          },
          error: (err) => {
            this.loadingPrefix = false;
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    });
  }

  private loadTopics() {
    this.forumService.forums$.pipe(take(1)).subscribe((exists) => {
      if (exists.length > 0) return;
      this.loadingTopic = true;
      this.forumService
        .listForum(true)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.loadingTopic = false;
            this.forumService.forums$.next(data);
          },
          error: (err) => {
            this.loadingTopic = false;
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    });
  }

  private patchFormValue() {
    if (!this.postId) return;
    this.postService
      .getPost(this.postId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.form.patchValue({ ...data });
          this.editor.setQuillContent(data.body);
          this.cdk.detectChanges();
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  private routerTracking() {
    if (this.formType === 'update') return;
    this.activatedRoute.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((paramMap) => {
        const topicId = paramMap.get('topicId');
        if (!topicId || !isGUID(topicId)) return;
        this.form.get('topicId')?.setValue(topicId);
      });
    this.form
      .get('topicId')
      ?.valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((topicId) => {
        this.commonService.navigateToCreatePost(topicId);
      });
  }
}
