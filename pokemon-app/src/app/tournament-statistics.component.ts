import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OnInit } from '@angular/core';
import { PokemonService } from './pokemon.service';  // Import PokemonService

@Component({
    selector: 'app-tournament-statistics',
    standalone: true,
    imports: [CommonModule, FormsModule],  // Include FormsModule here
    providers: [PokemonService],  // Provide PokemonService here
    templateUrl: './tournament-statistics.component.html'
})
export class TournamentStatisticsComponent implements OnInit {
    sortBy: string = 'wins';
    sortDirection: string = 'desc';
    tournamentStatistics: any[] = [];  // This holds the original list of tournament statistics
    sortedStatistics: any[] = [];  // This holds the sorted list

    constructor(private http: HttpClient) { }

    ngOnInit(): void {
        this.fetchStatistics();  // Fetch the initial data when the component loads
    }

    fetchStatistics() {
        const url = `https://localhost:7252/pokemon/tournament/statistics?sortBy=${this.sortBy}&sortDirection=${this.sortDirection}`;
        this.http.get<any[]>(url).subscribe({
            next: (data) => {
                this.tournamentStatistics = data;  // Store the original list
                this.sortStatistics();  // Sort the list based on the current direction
            },
            error: (error) => {
                console.error('Error fetching tournament statistics:', error);
            },
        });
    }

    // Sort the tournament statistics based on the sort direction
    sortStatistics() {
        this.sortedStatistics = [...this.tournamentStatistics];  // Copy the original list to sortedStatistics
        this.sortedStatistics.sort((a, b) => {
            if (this.sortDirection === 'asc') {
                return a[this.sortBy] - b[this.sortBy];  // Sort in ascending order
            } else {
                return b[this.sortBy] - a[this.sortBy];  // Sort in descending order
            }
        });
    }

    // This method will be called when the sortBy or sortDirection changes
    onSortChange() {
        this.sortStatistics();  // Re-sort the existing list with updated sort criteria
    }
}