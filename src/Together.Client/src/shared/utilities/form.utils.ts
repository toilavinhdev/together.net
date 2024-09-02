import {
  AbstractControl,
  UntypedFormGroup,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';

export function markFormDirty(form: UntypedFormGroup) {
  Object.values(form.controls).forEach((control) => {
    if (control.invalid) {
      control.markAsDirty();
      control.updateValueAndValidity({ onlySelf: true });
    }
  });
}

export class CustomValidators {
  static matchTo(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return !!control.parent &&
        !!control.parent.value &&
        control.value === (control.parent?.controls as any)[matchTo].value
        ? null
        : { matchTo: true };
    };
  }
}
