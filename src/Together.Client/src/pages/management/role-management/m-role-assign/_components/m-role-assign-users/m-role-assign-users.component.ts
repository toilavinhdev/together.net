import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AvatarComponent } from '@/shared/components/elements';
import { DropdownModule } from 'primeng/dropdown';
import { PrimeTemplate } from 'primeng/api';
import { BaseComponent } from '@/core/abstractions';
import { UserService } from '@/shared/services';
import {
  IListUserRequest,
  IUserViewModel,
} from '@/shared/entities/user.entities';
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { getErrorMessage } from '@/shared/utilities';
import { InputTextModule } from 'primeng/inputtext';
import { Button } from 'primeng/button';

@Component({
  selector: 'together-m-role-assign-users',
  standalone: true,
  imports: [
    AvatarComponent,
    DropdownModule,
    PrimeTemplate,
    FormsModule,
    InputTextModule,
    Button,
  ],
  templateUrl: './m-role-assign-users.component.html',
})
export class MRoleAssignUsersComponent extends BaseComponent implements OnInit {
  @Output() userIdChange = new EventEmitter<string>();

  @Input() userId = '';

  users: IUserViewModel[] = [];

  params: IListUserRequest = {
    pageIndex: 1,
    pageSize: 10,
    search: undefined,
  };

  search$ = new Subject<void>();

  loading = false;

  hasNextPage = true;

  constructor(private userService: UserService) {
    super();
  }

  ngOnInit() {
    this.loadUsers();
    this.searchDebounceTime();
  }

  private loadUsers() {
    if (!this.hasNextPage) return;
    this.loading = true;
    this.userService
      .listUser(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.loading = false;
          this.users =
            pagination.pageIndex === 1 ? result : [...this.users, ...result];
          this.hasNextPage = pagination.hasNextPage;
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  private searchDebounceTime() {
    this.search$.pipe(debounceTime(300)).subscribe(() => {
      this.params.pageIndex = 1;
      this.hasNextPage = true;
      this.loadUsers();
    });
  }

  listenScrollReachedBottom() {
    const container = document.querySelector(
      '#m-role-assign-users-dropdown-wrapper .p-dropdown-items-wrapper',
    );
    if (!container) return;
    container.addEventListener('scroll', () => {
      const reachedBottom =
        container.scrollTop + container.clientHeight >= container.scrollHeight;
      if (reachedBottom) {
        this.params.pageIndex++;
        this.loadUsers();
      }
    });
  }
}
