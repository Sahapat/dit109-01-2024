using System;
using Newtonsoft.Json;

namespace RockPaperScissors.Models
{
    [Serializable]
    public class RoomModel
    {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("player1")]
        public string Player1;
        [JsonProperty("player2")]
        public string Player2;
        [JsonProperty("p1_choice")]
        public string P1Choice;
        [JsonProperty("p2_choice")]
        public string P2Choice;
        [JsonProperty("is_close")]
        public bool IsClosed;
        [JsonProperty("is_game_finish")]
        public bool IsGameFinish;
        [JsonProperty("is_game_start")]
        public bool IsGameStart;
        [JsonProperty("winner")]
        public int Winner;
    }
}
