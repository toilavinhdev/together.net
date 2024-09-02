import {
  AbstractControl,
  ControlValueAccessor,
  NgControl,
} from '@angular/forms';
import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectorRef,
  Directive,
  inject,
  Injector,
  Input,
  Type,
} from '@angular/core';

@Directive()
export class BaseControl<T = string>
  implements ControlValueAccessor, AfterViewInit, AfterViewChecked
{
  @Input()
  disable = false;

  @Input()
  loading = false;

  @Input()
  placeholder = '';

  @Input()
  controlClass = '';

  @Input()
  wrapperClass = '';

  protected injector = inject(Injector);

  protected cdk = inject(ChangeDetectorRef);

  protected control!: AbstractControl;

  protected value!: T;

  onChange = (value: T) => {};

  onTouched = () => {};

  get isError(): boolean {
    return (
      !!this.control &&
      this.control.invalid &&
      (this.control.dirty || this.control.touched)
    );
  }

  ngAfterViewInit() {
    try {
      const ngControl = this.injector.get<NgControl>(
        NgControl as Type<NgControl>,
      );
      if (ngControl) {
        this.control = ngControl.control as AbstractControl;
      }
    } catch (err) {}
  }

  ngAfterViewChecked() {
    this.cdk.detectChanges();
  }

  /*
   * Implementation
   * */
  writeValue(value: T): void {
    this.value = value;
    this.onChange(value);
  }

  /*
   * Implementation
   * */
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  /*
   * Implementation
   * */
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
