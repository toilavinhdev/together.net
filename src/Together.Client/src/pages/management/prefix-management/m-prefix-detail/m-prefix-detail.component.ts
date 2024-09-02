import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { PrefixService, UserService } from '@/shared/services';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntil } from 'rxjs';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import {
  ContainerComponent,
  PrefixComponent,
} from '@/shared/components/elements';
import { InputTextModule } from 'primeng/inputtext';
import { AsyncPipe, JsonPipe, NgIf } from '@angular/common';
import { Button } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-m-prefix-detail',
  standalone: true,
  imports: [
    ContainerComponent,
    FormsModule,
    InputTextModule,
    NgIf,
    ReactiveFormsModule,
    Button,
    RouterLink,
    ColorPickerModule,
    JsonPipe,
    PrefixComponent,
    AsyncPipe,
  ],
  templateUrl: './m-prefix-detail.component.html',
})
export class MPrefixDetailComponent extends BaseComponent implements OnInit {
  form!: UntypedFormGroup;

  formType: 'create' | 'update' = 'create';

  prefixId = '';

  constructor(
    private prefixService: PrefixService,
    private formBuilder: FormBuilder,
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

  private getFormType() {
    this.activatedRoute.url.pipe(takeUntil(this.destroy$)).subscribe((url) => {
      if (url?.[0].path === 'create') {
        this.formType = 'create';
        this.commonService.title$.next('Tạo prefix');
      } else {
        this.formType = 'update';
        this.commonService.title$.next('Cập nhật prefix');
        this.prefixId = url?.[0].path;
        this.patchValueForm();
      }
    });
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      id: [null],
      name: [null, [Validators.required]],
      foreground: ['#FFFFFF', [Validators.required]],
      background: ['#4338CA', [Validators.required]],
    });
  }

  private patchValueForm() {
    this.prefixService
      .getPrefix(this.prefixId)
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
      this.prefixService
        .createPrefix(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Tạo prefix thành công',
            });
            this.router.navigate(['/management/prefix']).then();
          },
          error: (err) => {
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    } else {
      this.prefixService
        .updatePrefix(this.form.value)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Cập nhật  prefix thành công',
            });
            this.router.navigate(['/management/prefix']).then();
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
