import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { PrefixService, UserService } from '@/shared/services';
import { IPrefixViewModel } from '@/shared/entities/prefix.entities';
import { takeUntil } from 'rxjs';
import {
  PrefixComponent,
  TableCellDirective,
  TableColumnDirective,
  TableComponent,
} from '@/shared/components/elements';
import { Button } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { getErrorMessage } from '@/shared/utilities';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { policies } from '@/shared/constants';
import { AsyncPipe, NgIf } from '@angular/common';

@Component({
  selector: 'together-m-prefix-list',
  standalone: true,
  imports: [
    TableComponent,
    TableColumnDirective,
    Button,
    TableCellDirective,
    RouterLink,
    PrefixComponent,
    ConfirmDialogModule,
    NgIf,
    AsyncPipe,
  ],
  templateUrl: './m-prefix-list.component.html',
  providers: [ConfirmationService],
})
export class MPrefixListComponent extends BaseComponent implements OnInit {
  prefixes: IPrefixViewModel[] = [];

  constructor(
    private prefixService: PrefixService,
    private confirmationService: ConfirmationService,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.commonService.title$.next('Danh sách prefix');
    this.loadPrefixes();
  }

  private loadPrefixes() {
    this.prefixService
      .listPrefix()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.prefixes = data;
        },
        error: () => {
          this.commonService.toast$.next({
            type: 'error',
            message: 'Tải danh sách prefix thất bại',
          });
        },
      });
  }

  confirmDeletePrefixes(event: Event, prefixId: string, prefixName: string) {
    console.log(111);
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: `Dữ liệu của prefix '${prefixName}' khi xóa sẽ không thể hoàn tác`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.prefixService
          .deletePrefix(prefixId)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.commonService.toast$.next({
                type: 'success',
                message: 'Xóa prefix thành công',
              });
              this.prefixes = this.prefixes.filter((r) => r.id !== prefixId);
            },
            error: (err) => {
              this.commonService.toast$.next({
                type: 'error',
                message: getErrorMessage(err),
              });
            },
          });
      },
      reject: () => {},
    });
  }

  protected readonly policies = policies;
}
