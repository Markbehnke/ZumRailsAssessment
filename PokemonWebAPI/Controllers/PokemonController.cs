using Microsoft.AspNetCore.Mvc;
using PokemonWebAPI.Models;
using PokemonWebAPI.Services;
using PokemonWebAPI.DTO_Models;
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
        public async Task<IActionResult> GetTournamentStatistics([FromQuery] string sortBy, [FromQuery] string sortDirection)
        {
            await _pokemonService.SimulateTournament(sortBy, sortDirection);
            List<PokemonStatisticsDto> pokemons = _pokemonService.PokemonStatisticsList;
            if (pokemons == null || !pokemons.Any())
            {
                return NotFound("No Pokémon found. Please ensure the tournament has been simulated.");
            }
            return Ok(pokemons);
        }


    }

}
