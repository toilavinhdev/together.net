import {
  booleanAttribute,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { PaginatorModule } from 'primeng/paginator';
import { BaseComponent } from '@/core/abstractions';
import { reduce } from 'rxjs';

export interface IPaginatorChange {
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'together-paginator',
  standalone: true,
  imports: [PaginatorModule],
  templateUrl: './paginator.component.html',
})
export class PaginatorComponent extends BaseComponent {
  @Input()
  pageIndex = 1;

  @Input()
  pageSize = 5;

  @Input()
  totalRecord = 50;

  @Input({ transform: booleanAttribute })
  showCurrentPageReport = false;

  @Input()
  rowsPerPageOptions?: number[];

  @Output()
  paginationChange = new EventEmitter<IPaginatorChange>();

  onPageChange(event: any) {
    console.log(event);
    this.paginationChange.emit({
      pageIndex: event.page + 1,
      pageSize: event.rows,
    });
  }
}
