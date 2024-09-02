import {
  AfterViewInit,
  Component,
  ElementRef,
  forwardRef,
  Input,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import Quill from 'quill';
import { BaseControl } from '@/core/abstractions';
import { NgClass, NgIf, NgTemplateOutlet } from '@angular/common';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'together-editor',
  standalone: true,
  imports: [NgClass, NgTemplateOutlet, NgIf],
  templateUrl: './editor.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditorComponent),
      multi: true,
    },
  ],
})
export class EditorComponent extends BaseControl implements AfterViewInit {
  @ViewChild('editorContainer') editorContainer!: ElementRef;

  @Input() theme: 'bubble' | 'snow' = 'snow';

  @Input() toolbar = [
    [{ header: [1, 2, false] }],
    ['bold', 'italic', 'underline'],
    [{ list: 'ordered' }, { list: 'bullet' }],
    ['link', 'image', 'video'],
    [{ align: [] }],
    [{ color: [] }, { background: [] }],
    ['clean'],
  ];

  @Input() bottomTpl?: TemplateRef<any>;

  quill!: Quill;

  get quillContent() {
    if (!this.editorContainer) return '';
    return;
  }

  constructor() {
    super();
  }

  override ngAfterViewInit() {
    super.ngAfterViewInit();
    this.buildQuill();
  }

  private buildQuill() {
    if (!this.editorContainer) return;

    this.quill = new Quill(this.editorContainer.nativeElement, {
      theme: this.theme,
      placeholder: this.placeholder,
      readOnly: this.disable,
      modules: {
        syntax: false,
        toolbar: this.toolbar,
      },
    });

    this.quill.on('text-change', (delta, oldDelta, emitterSource) => {
      if (emitterSource === 'user') {
        this.writeValue(this.getQuillContent());
      }
    });
  }

  getQuillContent() {
    return this.quill.root.innerHTML;
  }

  setQuillContent(html: string) {
    this.quill.root.innerHTML = html;
  }
}
