import { Directive, TemplateRef } from '@angular/core';

@Directive({
  selector: '[togetherTableCell]',
  standalone: true,
})
export class TableCellDirective {
  constructor(public template: TemplateRef<any>) {}
}
