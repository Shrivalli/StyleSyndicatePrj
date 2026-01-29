import { Component } from '@angular/core';
import { StyleRequestComponent } from './components/style-request.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [StyleRequestComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Style Syndicate';
}
