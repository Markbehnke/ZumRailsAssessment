import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class PokemonService {
    private apiUrl = 'https://localhost:7252/pokemon/tournament/statistics';

    constructor(private http: HttpClient) { }

    getTournamentStatistics(sortBy: string, sortDirection: string): Observable<any[]> {
        const url = `${this.apiUrl}?sortBy=${sortBy}&sortDirection=${sortDirection}`;
        return this.http.get<any[]>(url);
    }
}
