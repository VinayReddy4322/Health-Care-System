import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.html'
})
export class Loader {
  @Input() label = 'Loading';
}
