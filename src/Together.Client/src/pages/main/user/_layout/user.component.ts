import { Component } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ProfileInfoComponent } from './_components/profile-info/profile-info.component';
import { ProfilePostListComponent } from './_components/profile-post-list/profile-post-list.component';
import { TabViewModule } from 'primeng/tabview';

@Component({
  selector: 'together-user',
  standalone: true,
  imports: [ProfileInfoComponent, ProfilePostListComponent, TabViewModule],
  templateUrl: './user.component.html',
})
export class UserComponent extends BaseComponent {}
