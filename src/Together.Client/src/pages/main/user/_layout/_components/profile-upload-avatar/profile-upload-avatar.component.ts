import { Component, OnInit } from '@angular/core';
import { FileUploadModule } from 'primeng/fileupload';
import { BaseComponent } from '@/core/abstractions';
import { TooltipModule } from 'primeng/tooltip';
import { FileService, UserService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';

@Component({
  selector: 'together-profile-upload-avatar',
  standalone: true,
  imports: [FileUploadModule, TooltipModule],
  templateUrl: './profile-upload-avatar.component.html',
})
export class ProfileUploadAvatarComponent
  extends BaseComponent
  implements OnInit
{
  constructor(
    private userService: UserService,
    private fileService: FileService,
  ) {
    super();
  }

  ngOnInit() {}

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.commonService.toast$.next({
      type: 'info',
      message: 'Đang tải ảnh lên server...',
    });
    this.fileService
      .uploadFile({
        file,
        bucket: 'v3-avatar',
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ url, publicId }) => {
          this.commonService.toast$.next({
            type: 'info',
            message: 'Tải ảnh thành công, đang cập nhật ảnh đại diện...',
          });
          this.userService
            .updateAvatar(url)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: () => {
                if (this.userService.me$.value?.avatar) {
                  this.fileService
                    .deleteFile(this.userService.me$.value.avatar)
                    .pipe(takeUntil(this.destroy$))
                    .subscribe();
                }
                this.updateAvatar(url);
                this.commonService.toast$.next({
                  type: 'success',
                  message: 'Cập nhật ảnh đại diện thành công',
                });
              },
              error: () => {
                this.commonService.toast$.next({
                  type: 'error',
                  message: 'Cập nhật ảnh đại diện thất bại',
                });
                this.fileService
                  .deleteFile(publicId)
                  .pipe(takeUntil(this.destroy$))
                  .subscribe();
              },
            });
        },
        error: () => {
          this.commonService.toast$.next({
            type: 'error',
            message: 'Tải ảnh lên server thất bại',
          });
        },
      });
  }

  private updateAvatar(url: string) {
    if (!this.userService.me$.value) return;
    this.userService.me$.next({ ...this.userService.me$.value, avatar: url });
    if (this.userService.me$.value.id === this.userService.user$.value?.id) {
      this.userService.user$.next({
        ...this.userService.user$.value,
        avatar: url,
      });
    }
  }
}
