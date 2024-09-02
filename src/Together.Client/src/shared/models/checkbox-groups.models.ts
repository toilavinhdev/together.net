export interface ICheckBoxGroup {
  label: string;
  description?: string;
  checked?: boolean;
  disable?: boolean;
  items: ICheckboxItem[];
}

export interface ICheckboxItem {
  label: string;
  value: string;
  checked?: boolean;
  disable?: boolean;
  description?: string;
}
