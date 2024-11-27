using Newtonsoft.Json;

namespace RockPaperScissors
{
    public class ChoiceModel
    {
        [JsonProperty("choice")]
        public string choice { get; set; }
    }
}
