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
        public async Task<IActionResult> GetTournamentStatistics([FromQuery] string sortBy = "", [FromQuery] string sortDirection = "desc")
        {
            try
            {
                await _pokemonService.SimulateTournament(sortBy, sortDirection);
                List<PokemonStatisticsDto> pokemons = _pokemonService.PokemonStatisticsList;
                if (pokemons == null || !pokemons.Any())
                {
                    return NotFound("No Pokemon found. Please try again.");
                }
                return Ok(pokemons);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("Missing required parameters.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "External API request failed. Please try again later.");
            }
            catch (TimeoutException ex)
            {
                return StatusCode(408, "The request timed out. Please try again later.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }

        }


    }

}
