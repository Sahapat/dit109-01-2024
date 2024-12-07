using System;
using Newtonsoft.Json;

namespace RockPaperScissors.Models
{
    [Serializable]
    public class UserModel
    {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("username")]
        public string Name;
    }
}
