import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { AuthService } from '@/shared/services';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { genderDropdownOptions, regexPatterns } from '@/shared/constants';
import {
  CustomValidators,
  getErrorMessage,
  markFormDirty,
} from '@/shared/utilities';
import { InputTextModule } from 'primeng/inputtext';
import { JsonPipe, NgIf } from '@angular/common';
import { PasswordModule } from 'primeng/password';
import { Button } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { MessagesModule } from 'primeng/messages';
import { MessageService } from 'primeng/api';
import { takeUntil } from 'rxjs';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'together-sign-up',
  standalone: true,
  imports: [
    InputTextModule,
    NgIf,
    PasswordModule,
    ReactiveFormsModule,
    Button,
    RouterLink,
    MessagesModule,
    JsonPipe,
    DropdownModule,
  ],
  templateUrl: './sign-up.component.html',
  providers: [MessageService],
})
export class SignUpComponent extends BaseComponent implements OnInit {
  protected readonly genderDropdownOptions = genderDropdownOptions;

  signUpForm!: UntypedFormGroup;

  loading = false;

  constructor(
    private authService: AuthService,
    private formBuilder: UntypedFormBuilder,
    private messageService: MessageService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
  }

  private buildForm() {
    this.signUpForm = this.formBuilder.group({
      userName: [
        null,
        [Validators.required, Validators.pattern(regexPatterns.userName)],
      ],
      email: [
        null,
        [Validators.required, Validators.pattern(regexPatterns.email)],
      ],
      gender: [null, [Validators.required]],
      password: [null, [Validators.required]],
      confirmPassword: [null, [CustomValidators.matchTo('password')]],
    });
  }

  onSubmit() {
    if (this.signUpForm.invalid) {
      markFormDirty(this.signUpForm);
      return;
    }
    this.loading = true;
    this.messageService.clear();
    this.authService
      .signUp(this.signUpForm.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loading = false;
          this.commonService.navigateToLogin();
          this.commonService.toast$.next({
            type: 'success',
            message: 'Đăng ký thành công',
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
