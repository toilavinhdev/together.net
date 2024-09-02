import {
  Directive,
  ElementRef,
  EventEmitter,
  HostListener,
  Output,
} from '@angular/core';

@Directive({
  selector: '[togetherClickOutside]',
  standalone: true,
})
export class ClickOutsideDirective {
  @Output() clickOutside: EventEmitter<any> = new EventEmitter();

  constructor(private elementRef: ElementRef) {}

  @HostListener('document:click', ['$event', '$event.target'])
  public onClick($event: any, $target: any) {
    const isClickedInside = this.elementRef.nativeElement.contains($target);

    if (isClickedInside) return;

    this.clickOutside.emit($event);
  }
}
