import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { UserService } from '@/shared/services';
import {
  IListUserRequest,
  IUserViewModel,
} from '@/shared/entities/user.entities';
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import {
  AvatarComponent,
  IPaginatorChange,
  PaginatorComponent,
  TableCellDirective,
  TableColumnDirective,
  TableComponent,
  UserStatusComponent,
} from '@/shared/components/elements';
import { Button } from 'primeng/button';
import { NgIf } from '@angular/common';
import { MUserListRolesDropdownComponent } from '@/pages/management/user-management/m-user-list/_components';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'together-m-user-list',
  standalone: true,
  imports: [
    TableComponent,
    Button,
    TableCellDirective,
    TableColumnDirective,
    AvatarComponent,
    UserStatusComponent,
    NgIf,
    PaginatorComponent,
    MUserListRolesDropdownComponent,
    InputTextModule,
    FormsModule,
  ],
  templateUrl: './m-user-list.component.html',
})
export class MUserListComponent extends BaseComponent implements OnInit {
  users: IUserViewModel[] = [];

  params: IListUserRequest = {
    pageIndex: 1,
    pageSize: 10,
    search: undefined,
    roleId: undefined,
  };

  search$ = new Subject<void>();

  totalUsers = 0;

  loading = false;

  constructor(private userService: UserService) {
    super();
  }

  ngOnInit() {
    this.commonService.title$.next('Danh sách thành viên');
    this.searchDebounceTime();
    this.loadUsers();
  }

  private loadUsers() {
    this.loading = true;
    this.userService
      .listUser(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.loading = false;
          this.users = result;
          this.totalUsers = pagination.totalRecord;
        },
        error: (err) => {
          this.loading = false;
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  onPaginationChange(event: IPaginatorChange) {
    const { pageIndex, pageSize } = event;
    this.params.pageIndex = pageIndex;
    this.params.pageSize = pageSize;
    this.loadUsers();
  }

  onRoleIdChange(roleId: string) {
    this.params.roleId = roleId;
    this.resetPagination();
    this.loadUsers();
  }

  private resetPagination() {
    this.totalUsers = 0;
    this.params.pageIndex = 1;
  }

  private searchDebounceTime() {
    this.search$
      .pipe(takeUntil(this.destroy$), debounceTime(300))
      .subscribe(() => {
        this.resetPagination();
        this.loadUsers();
      });
  }
}
