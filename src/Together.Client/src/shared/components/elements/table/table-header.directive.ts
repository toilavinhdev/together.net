import { Directive, TemplateRef } from '@angular/core';

@Directive({
  selector: '[togetherTableHeader]',
  standalone: true,
})
export class TableHeaderDirective {
  constructor(public template: TemplateRef<any>) {}
}
