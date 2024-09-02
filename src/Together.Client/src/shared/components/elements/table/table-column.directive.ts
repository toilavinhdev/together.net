import { ContentChild, Directive, Input } from '@angular/core';
import { TableHeaderDirective } from './table-header.directive';
import { TableCellDirective } from './table-cell.directive';

@Directive({
  selector: 'together-table-column',
  standalone: true,
})
export class TableColumnDirective {
  @ContentChild(TableHeaderDirective, { static: true })
  headerTpl?: TableHeaderDirective;

  @ContentChild(TableCellDirective, { static: true })
  cellTpl?: TableHeaderDirective;

  @Input()
  type: 'index' | 'text' | 'datetime' = 'text';

  @Input()
  key = '';

  @Input()
  title = '';

  @Input()
  headerClass = '';

  @Input()
  columnClass = '';

  constructor() {}
}
