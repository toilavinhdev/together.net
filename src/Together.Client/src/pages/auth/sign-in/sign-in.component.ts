import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { regexPatterns } from '@/shared/constants';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { JsonPipe, NgIf } from '@angular/common';
import { PasswordModule } from 'primeng/password';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { AuthService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { MessagesModule } from 'primeng/messages';
import { MessageService } from 'primeng/api';
import { ExternalAuthComponent } from '@/pages/auth/sign-in/_components';

@Component({
  selector: 'together-sign-in',
  standalone: true,
  imports: [
    InputTextModule,
    ReactiveFormsModule,
    ButtonModule,
    RouterLink,
    JsonPipe,
    PasswordModule,
    NgIf,
    MessagesModule,
    ExternalAuthComponent,
  ],
  templateUrl: './sign-in.component.html',
  providers: [MessageService],
})
export class SignInComponent extends BaseComponent implements OnInit {
  signInForm!: UntypedFormGroup;

  loading = false;

  error = '';

  constructor(
    private formBuilder: UntypedFormBuilder,
    private authService: AuthService,
    private messageService: MessageService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
  }

  private buildForm() {
    this.signInForm = this.formBuilder.group({
      email: [
        null,
        [Validators.required, Validators.pattern(regexPatterns.email)],
      ],
      password: [null, [Validators.required]],
    });
  }

  onSubmit() {
    if (this.signInForm.invalid) {
      markFormDirty(this.signInForm);
      return;
    }
    this.loading = true;
    this.messageService.clear();
    this.authService
      .signIn(this.signInForm.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.authService.setToken(data.accessToken, data.refreshToken);
          this.loading = false;
          this.commonService.navigateToMain();
          this.commonService.toast$.next({
            type: 'success',
            message: 'Đăng nhập thành công',
          });
        },
        error: (err) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: getErrorMessage(err),
            icon: undefined,
          });
        },
      });
  }
}
