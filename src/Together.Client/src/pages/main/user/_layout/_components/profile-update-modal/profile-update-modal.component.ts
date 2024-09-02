import { Component, OnInit } from '@angular/core';
import { Button } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { JsonPipe, NgIf } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { BaseComponent } from '@/core/abstractions';
import { UserService } from '@/shared/services';
import { genderDropdownOptions, regexPatterns } from '@/shared/constants';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { take, takeUntil } from 'rxjs';
import { getErrorMessage, markFormDirty } from '@/shared/utilities';
import { MessageService } from 'primeng/api';
import { MessagesModule } from 'primeng/messages';
import { DropdownModule } from 'primeng/dropdown';
import { IUpdateProfileRequest } from '@/shared/entities/user.entities';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'together-profile-update-modal',
  standalone: true,
  imports: [
    Button,
    DialogModule,
    InputTextModule,
    NgIf,
    ReactiveFormsModule,
    InputTextareaModule,
    MessagesModule,
    DropdownModule,
    JsonPipe,
    TooltipModule,
  ],
  templateUrl: './profile-update-modal.component.html',
  providers: [MessageService],
})
export class ProfileUpdateModalComponent
  extends BaseComponent
  implements OnInit
{
  protected readonly genderDropdownOptions = genderDropdownOptions;

  updateForm!: UntypedFormGroup;

  visible = false;

  loading = false;

  private originalFormValue!: IUpdateProfileRequest;

  get formIsChanged(): boolean {
    return (
      JSON.stringify(this.updateForm.value) !==
      JSON.stringify(this.originalFormValue)
    );
  }

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private messageService: MessageService,
  ) {
    super();
  }

  ngOnInit() {
    this.buildForm();
  }

  showUpdateModal() {
    this.visible = true;
    this.userService.user$.pipe(take(1)).subscribe((user) => {
      if (!user) return;
      this.updateForm.patchValue({ ...user });
      this.originalFormValue = this.updateForm.value;
    });
  }

  onSubmit() {
    if (this.updateForm.invalid) {
      markFormDirty(this.updateForm);
      return;
    }
    this.loading = true;
    this.messageService.clear();
    this.userService
      .updateProfile(this.updateForm.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loading = false;
          this.originalFormValue = this.updateForm.value;
          this.userService.user$.pipe(take(1)).subscribe((user) => {
            if (!user) return;
            this.userService.user$.next({ ...user, ...this.updateForm.value });
          });
          this.commonService.toast$.next({
            type: 'success',
            message: 'Cập nhật thông tin thành công',
          });
          this.visible = false;
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
    this.updateForm = this.formBuilder.group({
      userName: [
        null,
        [Validators.required, Validators.pattern(regexPatterns.userName)],
      ],
      gender: [null, [Validators.required]],
      fullName: [null],
      biography: [null],
    });
  }
}
