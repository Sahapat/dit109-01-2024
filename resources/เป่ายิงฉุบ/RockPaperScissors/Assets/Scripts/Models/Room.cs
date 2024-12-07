using System.Collections.Generic;

namespace RockPaperScissors.Models
{
    public class RoomModel
    {
        public string id  { get; set; }
        public string player1 { get; set; }
        public string Player2 { get; set; }
        public string p1_choice { get; set; }
        public string p2_choice { get; set; }
        public bool IsClosed { get; set; }
        public bool is_game_finish { get; set; }
        public bool is_game_start { get; set; }
        public int winner { get; set; }
    }

    public class RoomListResponse
    {
        public List<RoomModel> rooms { get; set; }
    }
}
