import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { AuthService } from '@/shared/services';
import { MessageService } from 'primeng/api';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { takeUntil } from 'rxjs';
import { MessagesModule } from 'primeng/messages';
import { Button } from 'primeng/button';
import { NgIf } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'together-forgot-password-submit',
  standalone: true,
  imports: [
    MessagesModule,
    Button,
    NgIf,
    ReactiveFormsModule,
    RouterLink,
    PasswordModule,
  ],
  templateUrl: './forgot-password-submit.component.html',
  providers: [MessageService],
})
export class ForgotPasswordSubmitComponent
  extends BaseComponent
  implements OnInit
{
  form!: UntypedFormGroup;

  loading = false;

  status: 'idle' | 'success' | 'failed' = 'idle';

  constructor(
    private authService: AuthService,
    private pmMessageService: MessageService,
    private formBuilder: UntypedFormBuilder,
    private activatedRoute: ActivatedRoute,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
    this.routerTracking();
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      userId: [null, [Validators.required]],
      token: [null, [Validators.required]],
      newPassword: [null, [Validators.required]],
    });
  }

  private routerTracking() {
    this.activatedRoute.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((paramMap) => {
        this.form.get('userId')?.patchValue(paramMap.get('userId'));
        this.form.get('token')?.patchValue(paramMap.get('token'));
        console.log(this.form.value);
      });
  }

  onSubmit() {
    if (this.form.invalid) {
      markFormDirty(this.form);
      return;
    }
    this.pmMessageService.clear();
    this.status = 'idle';
    this.loading = true;
    this.authService
      .submitForgotPasswordToken(this.form.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.status = 'success';
          this.loading = false;
          this.pmMessageService.add({
            severity: 'success',
            detail: 'Cập nhật mật khẩu thành công',
          });
        },
        error: (err) => {
          this.status = 'failed';
          this.loading = false;
          this.pmMessageService.add({
            severity: 'error',
            summary: getErrorMessage(err),
            icon: undefined,
          });
        },
      });
  }
}
