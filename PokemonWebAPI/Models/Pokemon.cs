using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PokemonWebAPI.Models
{
    public class Pokemon
    {
        public int PokemonId { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("types")]
        public JsonArray Types { get; set; }

        // Extract the first type's name using JsonNode
        public string Type => Types?.FirstOrDefault()?.AsObject()?["type"]?.AsObject()?["name"]?.ToString();
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties {  get; set; }
        [JsonPropertyName("base_experience")]
        public int Experience { get; set; }
    }

}
