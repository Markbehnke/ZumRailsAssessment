using PokemonWebAPI.Models;
using PokemonWebAPI.Controllers;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using PokemonWebAPI.DTO_Models;

namespace PokemonWebAPI.Services
{

    public interface IPokemonService
    {
        Task<Pokemon> FetchPokemonAPIAsync(int id);
        Task SimulateTournament();
        Task Fight(Pokemon pokemon1, Pokemon pokemon2);

    }

    public class PokemonService
    {
        private const int NUM_OF_POKEMON = 8;

        private readonly PokemonController _pokemonController;
        private readonly HttpClient _client;
        //{Key = Pokemon's type, Value = type it beats}
        private Dictionary<string, string> CounterType = new Dictionary<string, string>
        {
            {"water", "fire" },
            {"fire", "grass" },
            {"grass", "eletric" },
            {"eletric", "water" },
            {"ghost", "psychic" },
            {"psychic", "fighting" },
            {"fighting", "dark" },
            {"dark", "ghost" }
        };

        public List<Pokemon> PokemonList { get; set; } = new List<Pokemon>();

        public List<PokemonStatisticsDto> PokemonStatisticsList { get; set; }

        public PokemonService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.Timeout = TimeSpan.FromSeconds(120);
        }

        public async Task<List<PokemonStatisticsDto>> SimulateTournament(string sortBy, string sortDirection)
        {
            var validationResult = ValidateSortParameters(sortBy, sortDirection);
            if (validationResult != null)
            {
                return null;
            }

            await FetchPokemonsAsync();

            ConductFights();

            SortPokemons(sortBy, sortDirection);

            PokemonStatisticsList = MapToTournamentStatisticsDto(PokemonList);

            if (PokemonStatisticsList != null)
            {
                return PokemonStatisticsList;
            }
            return null;
        }

        private async Task FetchPokemonsAsync()
        {
            HashSet<int> randomIds = new HashSet<int>();
            var rand = new Random();

            //If NUM_OF_POKEMON ever scales to 151, there is a chance we enter an infinite loop.
            //We must handle that here.
            while (PokemonList.Count < NUM_OF_POKEMON)
            {
                int pokemonId = rand.Next(1, 151);
                Pokemon pokemon = await FetchPokemonAPIAsync(pokemonId);
                //If we have already seen this pokemon or the pokemon contains a type we do not support, generate a new one.
                if (randomIds.Contains(pokemonId) || !CounterType.ContainsKey(pokemon.Type))
                {
                    continue;
                }

                if (pokemon != null)
                {
                    PokemonList.Add(pokemon);
                    randomIds.Add(pokemonId);
                }
            }
        }

        private object GetSortValue(Pokemon pokemon, string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "wins" => pokemon.Wins,
                "losses" => pokemon.Losses,
                "ties" => pokemon.Ties,
                "name" => pokemon.Name,
                "id" => pokemon.PokemonId,
                _ => pokemon.Wins  // Default to sorting by wins
            };
        }

        private void SortPokemons(string sortBy, string sortDirection)
        {
            // Sort the Pokémon based on the specified criteria passed in by the API.
            PokemonList = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? PokemonList.OrderBy(p => GetSortValue(p, sortBy)).ToList()
                : PokemonList.OrderByDescending(p => GetSortValue(p, sortBy)).ToList();
        }

        private void ConductFights()
        {
            //Round robin style fighting.
            for (int i = 0; i < PokemonList.Count; i++)
            {
                for (int j = i + 1; j < PokemonList.Count; j++)
                {
                    //Skip if we are the same pokemon.
                    if (i == j)
                    {
                        continue;
                    }
                    Fight(PokemonList[i], PokemonList[j]);
                }
            }
        }

        private IActionResult ValidateSortParameters(string sortBy, string sortDirection)
        {
            var validSortFields = new HashSet<string> { "wins", "losses", "ties", "name", "id" };
            var validSortDirections = new HashSet<string> { "asc", "desc" };

            // if the 'sortBy' is invalid, we will return a bad request:
            if (string.IsNullOrWhiteSpace(sortBy) || !validSortFields.Contains(sortBy.ToLower()))
            {
                return new BadRequestObjectResult($"Invalid 'sortBy' parameter. Valid options are: {string.Join(", ", validSortFields)}.");
            }

            // if the 'sortDirection' is invalid, we will return a bad request:
            if (!validSortDirections.Contains(sortDirection.ToLower()))
            {
                return new BadRequestObjectResult($"Invalid 'sortDirection' parameter. Valid options are: {string.Join(", ", validSortDirections)}.");
            }

            // Return null if everything is valid
            return null;
        }

        public void Fight(Pokemon pokemon1,  Pokemon pokemon2)
        { 

            if (CounterType[pokemon1.Type].Equals(pokemon2.Type))
            {
                pokemon1.Wins++;
                pokemon2.Losses++;
            }
            else if (CounterType[pokemon2.Type].Equals(pokemon1.Type))
            {
                pokemon2.Wins++;
                pokemon1.Losses++; 
            }
            else
            {
            
                if (pokemon1.Experience == pokemon2.Experience)
                {
                    pokemon1.Ties++;
                    pokemon2.Ties++;
                }
                else if (pokemon1.Experience > pokemon2.Experience)
                {
                    pokemon1.Wins++;
                    pokemon2.Losses++;
                }
                else
                {
                    pokemon2.Wins++;
                    pokemon1.Losses++;
                }
            }

        }
        //This logic was placed in the Controller during the interview.
        //I moved it to the service layer for better seperation of concern between the controller and service layer.
        public async Task<Pokemon> FetchPokemonAPIAsync(int id)
        {
            var url = $"https://pokeapi.co/api/v2/pokemon/{id}";

            try
            { 
                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize and return the Pokémon data
                    return await response.Content.ReadFromJsonAsync<Pokemon>();
                }
                else
                {
                    var errorMessage = $"Failed to fetch Pokémon data for ID {id}. Status Code: {response.StatusCode}.";
                    return null; 
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unexpected error while fetching Pokémon data for ID {id}. Exception: {ex.Message}";
                return null;  
            }
        }

        private List<PokemonStatisticsDto> MapToTournamentStatisticsDto(List<Pokemon> pokemons)
        {
            // Map Pokemon to TournamentStatisticsDto
            return pokemons.Select(pokemon => new PokemonStatisticsDto
            {
                Id = pokemon.PokemonId,
                Name = pokemon.Name,
                Type = pokemon.Type,
                Wins = pokemon.Wins,
                Losses = pokemon.Losses,
                Ties = pokemon.Ties
            }).ToList();
        }

    }
}
