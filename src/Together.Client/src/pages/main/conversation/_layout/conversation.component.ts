import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ConversationListComponent } from '../conversation-list/conversation-list.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'together-conversation',
  standalone: true,
  imports: [ConversationListComponent, RouterOutlet],
  templateUrl: './conversation.component.html',
})
export class ConversationComponent extends BaseComponent implements OnInit {
  ngOnInit() {
    this.commonService.breadcrumb$.next([
      {
        title: 'Recent messages',
      },
    ]);
  }
}
