namespace PokemonWebAPI.DTO_Models
{
    //This will be the class we use to transmit from the API to the front end.
    public class PokemonStatisticsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
