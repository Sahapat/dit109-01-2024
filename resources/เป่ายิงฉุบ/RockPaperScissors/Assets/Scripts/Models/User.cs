using Newtonsoft.Json;

namespace RockPaperScissors.Models
{
    public class UserModel
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
        [JsonProperty("point")]
        public string point { get; set; }
    }
}
