import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../enviroments/enviroment';

interface Pokemon {
  id: number;
  name: string;
  wins: number;
  losses: number;
  ties: number;
  type: string;
  experience: number;
}

@Injectable({
  providedIn: 'root',
})
export class PokemonService {

  private apiUrl = 'https://localhost:7252/pokemon/tournament/statistics';

  constructor(private http: HttpClient) { }

  // Get sorted statistics of Pokémon
  getTournamentStatistics(sortBy: string, sortDirection: string): Observable<Pokemon[]> {
    const params = new HttpParams()
      .set('sortBy', sortBy)
      .set('sortDirection', sortDirection);
    return this.http.get<Pokemon[]>(this.apiUrl, { params });
  }
}
