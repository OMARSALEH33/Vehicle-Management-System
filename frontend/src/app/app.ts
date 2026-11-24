import { Component, } from '@angular/core';
import { RouterOutlet, } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  imports: [RouterOutlet,  FormsModule ,],
  selector: 'app-root', 
  styleUrls: ['./app.css'],
  templateUrl: './app.html',
})
export class App {
  
}
