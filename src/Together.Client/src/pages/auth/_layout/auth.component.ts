import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'together-auth',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './auth.component.html',
})
export class AuthComponent {}
