import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'together-forum-management',
  standalone: true,
  imports: [TranslateModule, RouterOutlet],
  templateUrl: './forum-management.component.html',
})
export class ForumManagementComponent {}
