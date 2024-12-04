import {
  AfterViewInit,
  Component,
  ElementRef,
  forwardRef,
  Input,
  OnDestroy,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import Quill from 'quill';
import { BaseControl } from '@/core/abstractions';
import { NgClass, NgIf, NgTemplateOutlet } from '@angular/common';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { TooltipModule } from 'primeng/tooltip';
import QuillResizeImage from 'quill-resize-image';
import { CloudinaryService } from '@/shared/services';
import { Subject } from 'rxjs';
import { base64ToFile, cloudinaryUploadImageXHR } from '@/shared/utilities';

// TODO: Thêm scripts trong angular.json xong register quill modules
Quill.register('modules/imageResize', QuillResizeImage);

@Component({
  selector: 'together-editor',
  standalone: true,
  imports: [NgClass, NgTemplateOutlet, NgIf, TooltipModule],
  templateUrl: './editor.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditorComponent),
      multi: true,
    },
  ],
})
export class EditorComponent
  extends BaseControl
  implements AfterViewInit, OnDestroy
{
  @ViewChild('editorContainer') editorContainer!: ElementRef;

  @Input() theme: 'bubble' | 'snow' = 'snow';

  @Input() bottomTpl?: TemplateRef<any>;

  private quillInstance!: Quill;

  tooltipPosition: 'right' | 'left' | 'top' | 'bottom' | undefined = 'top';

  private destroy$ = new Subject<void>();

  get quillContent() {
    if (!this.editorContainer) return '';
    return;
  }

  constructor(private cloudinaryService: CloudinaryService) {
    super();
  }

  override ngAfterViewInit() {
    super.ngAfterViewInit();
    this.buildQuill();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.destroy$.unsubscribe();
  }

  private buildQuill() {
    if (!this.editorContainer) return;
    this.quillInstance = new Quill(this.editorContainer.nativeElement, {
      theme: this.theme,
      placeholder: this.placeholder,
      readOnly: this.disable,
      modules: {
        syntax: false,
        toolbar: {
          container: '#editor-toolbar',
        },
        imageResize: {
          displaySize: true,
        },
      },
    });
    // this.addQuillImageHandler();
    this.quillInstance.on('text-change', (delta, oldDelta, emitterSource) => {
      if (emitterSource === 'user') {
        // TODO: Set control value
        this.writeValue(this.getQuillContent());
        // TODO: Convert base64 image to url
        const images = Array.from(
          document.querySelectorAll(
            '#editor-container img[src^="data:"]:not(.loading)',
          ),
        );
        for (const image of images) {
          image.classList.add('hidden');
          const base64 = image.getAttribute('src');
          cloudinaryUploadImageXHR(
            base64ToFile(base64!, Date.now().toString()),
            (response) => {
              image.setAttribute('src', response.url);
              image.classList.remove('hidden');
            },
          );
        }
      }
    });
  }

  getQuillContent() {
    return this.quillInstance.root.innerHTML;
  }

  setQuillContent(html: string) {
    this.quillInstance.root.innerHTML = html;
  }

  private addQuillImageHandler() {
    const toolbar = this.quillInstance.getModule('toolbar') as any;
    toolbar.addHandler('image', () => {
      let editorContainer = document.querySelector('#editor-container')!;
      let input = editorContainer.querySelector('input.ql-image[type=file]');
      if (input == null) {
        const input = document.createElement('input');
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'image/*');
        input.classList.add('ql-image');
        input.click();
        input.onchange = () => {
          const file = input.files![0];
          if (!file) return;
          cloudinaryUploadImageXHR(file, (response) => {
            const range = this.quillInstance.getSelection();
            this.quillInstance.insertEmbed(range!.index, 'image', response.url);
          });
        };
      }
      editorContainer.appendChild(input as Node);
    });
  }
}
