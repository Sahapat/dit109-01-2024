using Newtonsoft.Json;

namespace RockPaperScissors.Models
{
    public class UserModel
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("username")]
        public string Name { get; set; }
    }
}
