import {
  AfterViewChecked,
  Directive,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
} from '@angular/core';

@Directive({
  selector: '[togetherScrollReached]',
  standalone: true,
})
export class ScrollReachedDirective implements AfterViewChecked {
  @Output()
  scrollReached = new EventEmitter();

  @Input()
  scrollReachPosition: 'top' | 'bottom' = 'bottom';

  @Input()
  nearValue = 20;

  private emitted = false;

  constructor(private elementRef: ElementRef) {}

  ngAfterViewChecked() {}

  @HostListener('scroll')
  onScrolling() {
    const { offsetHeight, scrollTop, scrollHeight } =
      this.elementRef.nativeElement;

    switch (this.scrollReachPosition) {
      case 'top':
        if (scrollTop === 0) {
          this.scrollReached.emit();
          this.emitted = true;
        }
        break;
      case 'bottom':
        if (offsetHeight + scrollTop >= scrollHeight) {
          this.scrollReached.emit();
        }
        break;
    }
  }
}
