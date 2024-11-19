using System.Net;
using Moq;

using FluentAssertions;
using PokemonWebAPI.Models;
using PokemonWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
namespace PokemonWebAPI.UnitTests
{
    public class PokemonServicesTests
    {

        private PokemonService _pokemonService;

        public PokemonServicesTests()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
            var cacheMock = new MemoryCache(new MemoryCacheOptions());
            _pokemonService = new PokemonService(mockHttpClient, cacheMock);

        }

        [Fact]
        public async Task SimulateTournament_ValidParameters_ShouldReturnSortedStatistics()
        {
            var sortBy = "wins";
            var sortDirection = "asc";
            var pokemon1 = new Pokemon { PokemonId = 1, Name = "bulbasaur", Type = "grass", Wins = 0, Losses = 0, Ties = 0, Experience = 50 };
            var pokemon2 = new Pokemon { PokemonId = 2, Name = "hitmonchan", Type = "fighting", Wins = 0, Losses = 0, Ties = 0, Experience = 30 };
            var pokemon3 = new Pokemon { PokemonId = 3, Name = "venusaur", Type = "grass", Wins = 0, Losses = 0, Ties = 0, Experience = 100 };
            var pokemon4 = new Pokemon { PokemonId = 4, Name = "charizard", Type = "fire", Wins = 0, Losses = 0, Ties = 0, Experience = 200 };



            _pokemonService.PokemonList = new List<Pokemon> { pokemon1, pokemon2, pokemon3, pokemon4 };
            _pokemonService.ConductFights();
            _pokemonService.PokemonList[0].Wins.Should().Be(1);
            _pokemonService.PokemonList[1].Wins.Should().Be(0);
            _pokemonService.PokemonList[2].Wins.Should().Be(2);
            _pokemonService.PokemonList[3].Wins.Should().Be(3);
        }


        [Fact]
        public void Fight_PokemonShouldHaveCorrectResultsAfterFight_BasedOnExperience_Experience()
        {
            var pokemon1 = new Pokemon { PokemonId = 120, Name = "staryu", Type = "water" ,Wins = 0, Losses = 0, Ties = 0, Experience = 68 };
            var pokemon2 = new Pokemon { PokemonId = 1, Name = "bulbasaur", Type = "grass" , Wins = 0, Losses = 0, Ties = 0, Experience = 64 };
            _pokemonService.Fight(pokemon1, pokemon2);

            pokemon1.Wins.Should().Be(1);
            pokemon2.Losses.Should().Be(1);
        }

        //Check for ties
        [Fact]
        public void Fight_PokemonShouldHaveCorrectResultsAfterFight_BasedOnExperience_Ties()
        {
            var pokemon1 = new Pokemon { PokemonId = 120, Name = "staryu", Type = "water", Wins = 0, Losses = 0, Ties = 0, Experience = 68 };
            var pokemon2 = new Pokemon { PokemonId = 1, Name = "bulbasaur", Type = "grass", Wins = 0, Losses = 0, Ties = 0, Experience = 68 };

            _pokemonService.Fight(pokemon1, pokemon2);

            pokemon1.Ties.Should().Be(1);
            pokemon2.Ties.Should().Be(1);

            pokemon1.Wins.Should().Be(0);
            pokemon2.Losses.Should().Be(0);
        }

        [Fact]
        public void ValidateSortParameters_ShouldReturnBadRequest_WhenSortByIsInvalid()
        {

            var sortBy = "invalid";
            var sortDirection = "asc";

            Action act = () => _pokemonService.ValidateSortParameters(sortBy, sortDirection);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ValidateSortParameters_ShouldReturnBadRequest_WhenSortDirectionIsInvalid()
        {
            var sortBy = "wins";
            var sortDirection = "invalid";

            Action act = () => _pokemonService.ValidateSortParameters(sortBy, sortDirection);
            act.Should().Throw<ArgumentException>();  
        }
    }
}
