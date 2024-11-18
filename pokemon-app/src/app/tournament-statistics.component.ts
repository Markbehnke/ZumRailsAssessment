import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { PokemonService } from './pokemon.service';

@Component({
    selector: 'app-tournament-statistics',
    standalone: true, // Indicates this is a standalone component
    imports: [CommonModule, FormsModule],
    templateUrl: './tournament-statistics.component.html'
})
export class TournamentStatisticsComponent {
    pokemons: any[] = [];
    sortBy: string = 'wins';
    sortDirection: string = 'desc';

    constructor(private pokemonService: PokemonService) { }

    ngOnInit(): void {
        this.fetchStatistics();
    }

    fetchStatistics(): void {
        this.pokemonService.getTournamentStatistics(this.sortBy, this.sortDirection).subscribe(
            (data: any[]) => {
                this.pokemons = data;
            },
            (error) => {
                console.error('Error fetching tournament statistics:', error);
            }
        );
    }
}
