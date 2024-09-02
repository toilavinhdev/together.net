import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { RoleService, UserService } from '@/shared/services';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntil } from 'rxjs';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { ContainerComponent } from '@/shared/components/elements';
import { Button } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { AsyncPipe, JsonPipe, NgIf } from '@angular/common';
import { CheckboxGroupsComponent } from '@/shared/components/controls';
import {
  ICheckBoxGroup,
  ICheckboxItem,
} from '@/shared/models/checkbox-groups.models';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-m-role-detail',
  standalone: true,
  imports: [
    ContainerComponent,
    Button,
    InputTextModule,
    NgIf,
    ReactiveFormsModule,
    RouterLink,
    JsonPipe,
    CheckboxGroupsComponent,
    AsyncPipe,
  ],
  templateUrl: './m-role-detail.component.html',
})
export class MRoleDetailComponent extends BaseComponent implements OnInit {
  form!: UntypedFormGroup;

  formType: 'create' | 'update' = 'create';

  roleId = '';

  groups: ICheckBoxGroup[] = roleClaimCheckboxGroups;

  constructor(
    private formBuilder: UntypedFormBuilder,
    private roleService: RoleService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
    this.getFormType();
  }

  override ngOnDestroy() {
    super.ngOnDestroy();
  }

  private getFormType() {
    this.activatedRoute.url.pipe(takeUntil(this.destroy$)).subscribe((url) => {
      if (url?.[0].path === 'create') {
        this.formType = 'create';
        this.commonService.title$.next('Thêm vai trò');
      } else {
        this.formType = 'update';
        this.commonService.title$.next('Cập nhật vai trò');
        this.roleId = url?.[0].path;
        this.patchValueForm();
      }
    });
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      id: [null],
      name: [null, [Validators.required]],
      description: [null],
      claims: [[], [Validators.required]],
    });
  }

  private patchValueForm() {
    this.roleService
      .getRole(this.roleId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.form.patchValue({ ...data });
          if (data.claims.length) {
            this.groups = this.groups.map((group) => ({
              ...group,
              items: group.items.map((item) =>
                data.claims.includes(item.value) || data.claims.includes('All')
                  ? ({ ...item, checked: true } as ICheckboxItem)
                  : item,
              ),
            }));
            this.groups = this.groups.map((group) =>
              group.items.every((item) => item.checked)
                ? { ...group, checked: true }
                : group,
            );
          }
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
      this.roleService
        .createRole(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Tạo vai trò thành công',
            });
            this.router.navigate(['/management/role']).then();
          },
          error: (err) => {
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    } else {
      this.roleService
        .updateRole(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Cập nhật vai trò thành công',
            });
            this.router.navigate(['/management/role']).then();
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

const roleClaimCheckboxGroups: ICheckBoxGroup[] = [
  {
    label: 'Management',
    items: [
      {
        label: policies.Management.Access,
        value: policies.Management.Access,
        description: 'Có thể truy cập trang quản lý Together.NET',
      },
      {
        label: policies.Management.ViewDashboard,
        value: policies.Management.ViewDashboard,
        description: 'Có thể xem trang thống kê đầy đủ',
      },
    ],
  },
  {
    label: 'User',
    description: 'Tất cả quyền của người dùng',
    items: [
      {
        label: policies.User.Get,
        value: policies.User.Get,
        description: 'Có thể xem trang cá nhân thành viên',
      },
      {
        label: policies.User.List,
        value: policies.User.List,
        description: 'Có thể xem danh sách thành viên',
      },
    ],
  },
  {
    label: 'Role',
    description: 'Tất cả quyền của vai trò',
    items: [
      {
        label: 'Role:View',
        value: 'Role:View',
        description: 'Có thể xem được vai trò',
      },
      {
        label: 'Role:Create',
        value: 'Role:Create',
        description: 'Có thể tạo vai trò',
      },
      {
        label: 'Role:Update',
        value: 'Role:Update',
        description: 'Có thể cập nhật vai trò',
      },
      {
        label: 'Role:Delete',
        value: 'Role:Delete',
        description: 'Có thể xóa vai trò',
      },
      {
        label: 'Role:Assign',
        value: 'Role:Assign',
        description: 'Có thể gán vai trò cho người dùng',
      },
    ],
  },
  {
    label: 'Forum',
    description: 'Tất cả quyền của diễn đàn',
    items: [
      {
        label: 'Forum:View',
        value: 'Forum:View',
        description: 'Có thể xem diễn đàn',
      },
      {
        label: 'Forum:Create',
        value: 'Forum:Create',
        description: 'Có thể tạo diễn đàn',
      },
      {
        label: 'Forum:Update',
        value: 'Forum:Update',
        description: 'Có thể cập nhật diễn đàn',
      },
      {
        label: 'Forum:Delete',
        value: 'Forum:Delete',
        description: 'Có thể xóa diễn đàn',
      },
    ],
  },
  {
    label: 'Topic',
    description: 'Tất cả quyền của chủ đề',
    items: [
      {
        label: 'Topic:View',
        value: 'Topic:View',
        description: 'Có thể xem chủ đề',
      },
      {
        label: 'Topic:Create',
        value: 'Topic:Create',
        description: 'Có thể tạo chủ đề',
      },
      {
        label: 'Topic:Update',
        value: 'Topic:Update',
        description: 'Có thể cập nhật chủ đề',
      },
      {
        label: 'Topic:Delete',
        value: 'Topic:Delete',
        description: 'Có thể xóa chủ đề',
      },
    ],
  },
  {
    label: 'Prefix',
    description: 'Tất cả quyền của prefix',
    items: [
      {
        label: 'Prefix:View',
        value: 'Prefix:View',
        description: 'Có thể xem prefix',
      },
      {
        label: 'Prefix:Create',
        value: 'Prefix:Create',
        description: 'Có thể tạo prefix',
      },
      {
        label: 'Prefix:Update',
        value: 'Prefix:Update',
        description: 'Có thể cập nhật prefix',
      },
      {
        label: 'Prefix:Delete',
        value: 'Prefix:Delete',
        description: 'Có thể xóa prefix',
      },
    ],
  },
  {
    label: 'Post',
    description: 'Tất cả quyền của bài viết',
    items: [
      {
        label: 'Post:View',
        value: 'Post:View',
        description: 'Có thể xem bài viết',
      },
      {
        label: 'Post:Create',
        value: 'Post:Create',
        description: 'Có thể tạo bài viết',
      },
      {
        label: 'Post:Update',
        value: 'Post:Update',
        description: 'Có thể cập nhật bài viết',
      },
      {
        label: 'Post:Delete',
        value: 'Post:Delete',
        description: 'Có thể xóa bài viết',
      },
      {
        label: 'Post:Vote',
        value: 'Post:Vote',
        description: 'Có thể vote bài viết',
      },
    ],
  },
  {
    label: 'Reply',
    description: 'Tất cả quyền của phản hồi',
    items: [
      {
        label: 'Reply:View',
        value: 'Reply:View',
        description: 'Có thể xem phản hồi',
      },
      {
        label: 'Reply:Create',
        value: 'Reply:Create',
        description: 'Có thể tạo phản hồi',
      },
      {
        label: 'Reply:Update',
        value: 'Reply:Update',
        description: 'Có thể cập nhật phản hồi',
      },
      {
        label: 'Reply:Delete',
        value: 'Reply:Delete',
        description: 'Có thể xóa phản hồi',
      },
      {
        label: 'Reply:Vote',
        value: 'Reply:Vote',
        description: 'Có thể vote phản hồi',
      },
    ],
  },
];
