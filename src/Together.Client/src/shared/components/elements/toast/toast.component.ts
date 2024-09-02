import { Component, OnInit } from '@angular/core';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { BaseComponent } from '@/core/abstractions';
import { NgIf } from '@angular/common';
import { IToast } from '@/shared/models/toast.models';

@Component({
  selector: 'together-toast',
  standalone: true,
  imports: [ToastModule, NgIf],
  templateUrl: './toast.component.html',
  providers: [MessageService],
})
export class ToastComponent extends BaseComponent implements OnInit {
  model: IToast | undefined;

  constructor(private messageService: MessageService) {
    super();
  }

  ngOnInit() {
    this.commonService.toast$.subscribe((model) => {
      if (!model) return;
      this.model = model;
      this.messageService.add({
        severity: model.type,
        summary: model.message,
      });
    });
  }
}
