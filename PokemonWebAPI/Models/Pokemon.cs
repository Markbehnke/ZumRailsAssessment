using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PokemonWebAPI.Models
{
    public class Pokemon
    {
        [JsonPropertyName("id")]
        public int PokemonId { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("types")]
        public List<TypeWrapper> Types { get; set; }

        // Helper property to directly set the type as a string
        public string Type
        {
            get => Types?.FirstOrDefault()?.Type?.Name;
            set
            {
                if (Types == null) Types = new List<TypeWrapper>();
                if (Types.Count == 0) Types.Add(new TypeWrapper());
                Types[0].Type = new TypeObject { Name = value };
            }
        }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties {  get; set; }
        [JsonPropertyName("base_experience")]
        public int Experience { get; set; }
    }

    public class TypeWrapper
{
    [JsonPropertyName("type")]
    public TypeObject Type { get; set; }
}

public class TypeObject
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

}
