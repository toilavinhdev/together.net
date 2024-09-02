import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { BaseComponent } from '@/core/abstractions';
import { regexPatterns } from '@/shared/constants';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { AuthService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { InputTextModule } from 'primeng/inputtext';
import { NgIf } from '@angular/common';
import { MessagesModule } from 'primeng/messages';
import { MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'together-forgot-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    InputTextModule,
    NgIf,
    MessagesModule,
    Button,
    RouterLink,
  ],
  templateUrl: './forgot-password.component.html',
  providers: [MessageService],
})
export class ForgotPasswordComponent extends BaseComponent implements OnInit {
  form!: UntypedFormGroup;

  loading = false;

  status: 'idle' | 'success' | 'failed' = 'idle';

  constructor(
    private formBuilder: UntypedFormBuilder,
    private authService: AuthService,
    private pmMessageService: MessageService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      email: [
        null,
        [Validators.required, Validators.pattern(regexPatterns.email)],
      ],
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
      .forgotPassword(this.form.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.status = 'success';
          this.loading = false;
          this.pmMessageService.add({
            severity: 'success',
            detail: 'Vui lòng kiểm tra email của bạn',
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
