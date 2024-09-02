import {
  booleanAttribute,
  Component,
  ContentChild,
  ContentChildren,
  EventEmitter,
  Input,
  Output,
  QueryList,
  TemplateRef,
} from '@angular/core';
import {
  TableModule,
  TableRowCollapseEvent,
  TableRowExpandEvent,
} from 'primeng/table';
import {
  SvgIconComponent,
  TableColumnDirective,
} from '@/shared/components/elements';
import {
  DatePipe,
  NgClass,
  NgForOf,
  NgIf,
  NgSwitch,
  NgSwitchCase,
  NgTemplateOutlet,
} from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { BaseComponent } from '@/core/abstractions';
import { Button } from 'primeng/button';
import { DragDropModule } from 'primeng/dragdrop';

@Component({
  selector: 'together-table',
  standalone: true,
  imports: [
    TableModule,
    NgIf,
    NgTemplateOutlet,
    NgForOf,
    SvgIconComponent,
    NgSwitch,
    NgSwitchCase,
    DatePipe,
    TranslateModule,
    Button,
    DragDropModule,
    NgClass,
  ],
  templateUrl: './table.component.html',
})
export class TableComponent<T> extends BaseComponent {
  @ContentChildren(TableColumnDirective)
  columns!: QueryList<TableColumnDirective>;

  @Input()
  data: T[] = [];

  @Input()
  dataKey: string = 'id';

  @Input({ transform: booleanAttribute })
  loading = false;

  @Input({ transform: booleanAttribute })
  striped = false;

  @Input({ transform: booleanAttribute })
  grid = true;

  @Input({ transform: booleanAttribute })
  rowHover = true;

  @Input()
  scrollHeight?: string;

  @Input()
  nestedTableTpl?: TemplateRef<TableComponent<T>>;

  @Input()
  rowDraggable?: string;

  @Input()
  rowDragEffect?:
    | 'none'
    | 'copy'
    | 'copyLink'
    | 'copyMove'
    | 'link'
    | 'linkMove'
    | 'move'
    | 'all'
    | 'uninitialized';

  @Output()
  rowExpand = new EventEmitter<string>();

  @Output()
  rowCollapse = new EventEmitter<string>();

  @Output()
  onRowDragStart = new EventEmitter<T>();

  @Output()
  onRowDragEnd = new EventEmitter<T>();

  expandedRows = {};

  get classes() {
    const classes = [];
    if (this.striped) classes.push('p-datatable-striped');
    if (this.grid) classes.push('p-datatable-gridlines');
    return classes.reduce((acc, cur) => acc + ' ' + cur, '');
  }

  onRowExpand(event: TableRowExpandEvent) {
    this.rowExpand.emit(event.data[this.dataKey]);
  }

  onRowCollapse(event: TableRowCollapseEvent) {
    this.rowCollapse.emit(event.data[this.dataKey]);
  }
}
