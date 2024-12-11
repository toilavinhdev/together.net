import { BaseComponent } from '@/core/abstractions';
import { environment } from '@/environments/environment';
import {
  ScrollTopComponent,
  SpinnerComponent,
  SvgDefinitionsComponent,
  ToastComponent,
} from '@/shared/components/elements';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PrimeNGConfig } from 'primeng/api';
import { takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    SvgDefinitionsComponent,
    ToastComponent,
    SpinnerComponent,
    TranslateModule,
    ScrollTopComponent,
  ],
  templateUrl: './app.component.html',
})
export class AppComponent
  extends BaseComponent
  implements OnInit, AfterViewInit
{
  @ViewChild('audio') audioElement!: ElementRef<HTMLAudioElement>;

  constructor(
    private primengConfig: PrimeNGConfig,
    private translateService: TranslateService,
  ) {
    super();
  }

  ngOnInit() {
    this.setLanguages();
    this.configPrimeNG();
    this.audioHandler();
  }

  ngAfterViewInit() {}

  private setLanguages() {
    this.translateService.setDefaultLang(environment.lang);
    this.translateService.use(this.commonService.getCurrentLanguage());
  }

  private configPrimeNG() {
    this.primengConfig.ripple = true;
    this.primengConfig.zIndex = {
      modal: 101, // dialog, sidebar
      overlay: 100, // dropdown, overlay panel
      menu: 10, // overlay menus
      tooltip: 10, // tooltip
    };
  }

  private audioHandler() {
    this.commonService.audio$
      .pipe(takeUntil(this.destroy$))
      .subscribe((src) => {
        this.audioElement.nativeElement.src = src;
        this.audioElement.nativeElement.load();
        this.audioElement.nativeElement.play();
      });
  }
}
