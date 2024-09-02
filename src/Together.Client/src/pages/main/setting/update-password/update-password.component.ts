import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  FormsModule,
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import {
  CustomValidators,
  getErrorMessage,
  markFormDirty,
} from '@/shared/utilities';
import { UserService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { InputTextModule } from 'primeng/inputtext';
import { NgIf } from '@angular/common';
import { PasswordModule } from 'primeng/password';
import { Button } from 'primeng/button';
import { MessagesModule } from 'primeng/messages';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'together-update-password',
  standalone: true,
  imports: [
    FormsModule,
    InputTextModule,
    NgIf,
    PasswordModule,
    ReactiveFormsModule,
    Button,
    MessagesModule,
  ],
  templateUrl: './update-password.component.html',
  providers: [MessageService],
})
export class UpdatePasswordComponent extends BaseComponent implements OnInit {
  form!: UntypedFormGroup;

  loading = false;

  constructor(
    private formBuilder: UntypedFormBuilder,
    private userService: UserService,
    private messageService: MessageService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
  }

  onSubmit() {
    if (this.form.invalid) {
      markFormDirty(this.form);
      return;
    }
    this.loading = true;
    this.messageService.clear();
    this.userService
      .updatePassword(this.form.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loading = false;
          this.commonService.toast$.next({
            type: 'success',
            message: 'Cập nhật mật khẩu thành công',
          });
          this.form.reset();
        },
        error: (err) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            detail: getErrorMessage(err),
          });
        },
      });
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      currentPassword: [null, [Validators.required]],
      newPassword: [null, [Validators.required]],
      confirmNewPassword: [
        null,
        [Validators.required, CustomValidators.matchTo('newPassword')],
      ],
    });
  }
}
