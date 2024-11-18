using PokemonWebAPI.Models;
using PokemonWebAPI.Controllers;

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

        public PokemonService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.Timeout = TimeSpan.FromSeconds(120);
        }

        public async Task SimulateTournament()
        {
            var rand = new Random();
            HashSet<int> randomIds = new HashSet<int>();

            //If NUM_OF_POKEMON ever scales to 151, there is a chance we enter an infinite loop.
            //We must handle that here.
            while (PokemonList.Count < NUM_OF_POKEMON) 
            {
                int pokemonId = rand.Next(1, 151);  
                if (randomIds.Contains(pokemonId))
                {
                    continue;
                }
                Pokemon pokemon = await FetchPokemonAPIAsync(pokemonId);
                if (pokemon != null) 
                {
                    PokemonList.Add(pokemon);
                    randomIds.Add(pokemonId);
                }
            }

            //Round robin style fighting.
            for (int i = 0; i < PokemonList.Count; i++)
            {
                for(int j = i + 1; j < PokemonList.Count; j++)
                {
                    //Skip if we are the same pokemon.
                    if(i == j)
                    {
                        continue;
                    }
                    Fight(PokemonList[i], PokemonList[j]);
                }
            }
            
        }

        public void Fight(Pokemon pokemon1,  Pokemon pokemon2)
        {
            //Safely ensure that both pokemon have a valid type
            if(!CounterType.ContainsKey(pokemon1.Type) || !CounterType.ContainsKey(pokemon2.Type))
            {
                return;
            }

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
                    // Log or handle non-success status codes (e.g., 404, 500)
                    var errorMessage = $"Failed to fetch Pokémon data for ID {id}. Status Code: {response.StatusCode}.";
                    Console.WriteLine(errorMessage); // You can log this error or throw a custom exception
                    return null;  // Return null if the request fails
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle network-related exceptions
                var errorMessage = $"Network error while fetching Pokémon data for ID {id}. Exception: {ex.Message}";
                Console.WriteLine(errorMessage); // Log the error
                return null;  // Return null if there's an error
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                var errorMessage = $"Unexpected error while fetching Pokémon data for ID {id}. Exception: {ex.Message}";
                Console.WriteLine(errorMessage); // Log the error
                return null;  // Return null if there's an error
            }
        }



    }
}
