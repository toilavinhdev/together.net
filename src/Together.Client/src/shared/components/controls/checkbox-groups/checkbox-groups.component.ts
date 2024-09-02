import {
  Component,
  EventEmitter,
  forwardRef,
  Input,
  Output,
} from '@angular/core';
import { BaseControl } from '@/core/abstractions';
import { CheckboxModule } from 'primeng/checkbox';
import { ICheckBoxGroup } from '@/shared/models/checkbox-groups.models';
import { JsonPipe, NgClass, NgForOf } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'together-checkbox-groups',
  standalone: true,
  imports: [CheckboxModule, NgClass, NgForOf, FormsModule, JsonPipe],
  templateUrl: './checkbox-groups.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxGroupsComponent),
      multi: true,
    },
  ],
})
export class CheckboxGroupsComponent extends BaseControl<string[] | number[]> {
  @Input()
  groups: ICheckBoxGroup[] = [];

  @Output()
  groupsChange = new EventEmitter<ICheckBoxGroup[]>();

  onGroupCheck(idx: number, value: boolean) {
    this.groups.forEach((group, index) => {
      if (idx !== index) return;
      group.items = group.items.map((item) => ({ ...item, checked: value }));
    });
    this.updateValue();
    this.groupsChange.emit(this.groups);
  }

  onItemCheck(groupIdx: number, idx: number, value: boolean) {
    this.groups.forEach((group, index) => {
      if (groupIdx !== index) return;
      group.items = group.items.map((item, index) =>
        index === idx ? { ...item, checked: value } : item,
      );
      group.checked = group.items.every((item) => item.checked);
    });
    this.updateValue();
    this.groupsChange.emit(this.groups);
  }

  updateValue() {
    const value = this.groups
      .flatMap((group) => group.items)
      .filter((item) => item.checked)
      .map((item) => item.value);
    this.writeValue(value); // self
    this.onChange(value); // form group
  }
}
