import { Component } from '@angular/core';
import { TournamentStatisticsComponent } from './tournament-statistics.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TournamentStatisticsComponent],
  templateUrl: './app.component.html'
})
export class AppComponent { }
