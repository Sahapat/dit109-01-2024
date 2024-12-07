using System;
using Newtonsoft.Json;

namespace RockPaperScissors
{
    [Serializable]
    public class ChoiceModel
    {
        [JsonProperty("choice")]
        public string choice;
    }
}
