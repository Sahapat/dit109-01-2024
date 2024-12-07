using Newtonsoft.Json;

namespace RockPaperScissors.Models
{
    public class RoomModel
    {
        [JsonProperty("id")]
        public string ID  { get; set; }
        [JsonProperty("player1")]
        public string Player1 { get; set; }
        [JsonProperty("player2")]
        public string Player2 { get; set; }
        [JsonProperty("p1_choice")]
        public string P1Choice { get; set; }
        [JsonProperty("p2_choice")]
        public string P2Choice { get; set; }
        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }
        [JsonProperty("is_game_start")]
        public bool IsGameStart { get; set; }
        [JsonProperty("winner")]
        public int Winner { get; set; }
    }
}
