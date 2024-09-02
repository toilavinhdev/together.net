import { Component, Input } from '@angular/core';
import { EUserStatus } from '@/shared/enums';
import { TagModule } from 'primeng/tag';
import { NgSwitch, NgSwitchCase } from '@angular/common';

@Component({
  selector: 'together-user-status',
  standalone: true,
  imports: [TagModule, NgSwitch, NgSwitchCase],
  template: `
    <ng-container [ngSwitch]="status">
      <p-tag
        *ngSwitchCase="EUserStatus.Active"
        severity="success"
        value="Hoạt động"
      />
      <p-tag
        *ngSwitchCase="EUserStatus.Banned"
        severity="danger"
        value="Bị cấm"
      />
    </ng-container>
  `,
})
export class UserStatusComponent {
  @Input()
  status: EUserStatus = EUserStatus.Active;
  protected readonly EUserStatus = EUserStatus;
}
