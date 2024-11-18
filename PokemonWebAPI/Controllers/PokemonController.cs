using Microsoft.AspNetCore.Mvc;
using PokemonWebAPI.Models;
using PokemonWebAPI.Services;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
namespace PokemonWebAPI.Controllers
{
    [ApiController]
    [Route("pokemon/tournament")]
    public class PokemonController : ControllerBase
    {

        private readonly PokemonService _pokemonService;

        // Inject the PokemonService into the controller
        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;

        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetTournamentStatistics([FromQuery] string sortBy = "wins", [FromQuery] string sortDirection = "desc")
        {
            var validationResult = ValidateSortParameters(sortBy, sortDirection);
            if (validationResult != null)
            {
                return validationResult;
            }
            await _pokemonService.SimulateTournament();
            List<Pokemon> pokemons = _pokemonService.PokemonList;

            if (pokemons == null || !pokemons.Any())
            {
                return NotFound("No Pokémon found. Please ensure the tournament has been simulated.");
            }

            // Sort the Pokemon based on the specified criteria
            var sortedPokemons = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? pokemons.OrderBy(p => GetSortValue(p, sortBy)).ToList()
                : pokemons.OrderByDescending(p => GetSortValue(p, sortBy)).ToList();

            return Ok(sortedPokemons);
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

        private IActionResult ValidateSortParameters(string sortBy, string sortDirection)
        {
            var validSortFields = new HashSet<string> { "wins", "losses", "ties", "name", "id" };
            var validSortDirections = new HashSet<string> { "asc", "desc" };

            // if the 'sortFields' is invalid, we will return a bad request:r
            if (string.IsNullOrWhiteSpace(sortBy) || !validSortFields.Contains(sortBy.ToLower()))
            {
                return BadRequest($"Invalid 'sortBy' parameter. Valid options are: {string.Join(", ", validSortFields)}.");
            }

            // if the 'sortDirection' is invalid, we will return a bad request:
            if (!validSortDirections.Contains(sortDirection.ToLower()))
            {
                return BadRequest("The 'sortDirection' parameter must be either 'asc' or 'desc'.");
            }

            // Return null if everything is valid
            return null;
        }



    }





}
