using UnityEngine;
using Cysharp.Threading.Tasks;
using InspireTale.Utils;

namespace RockPaperScissors
{
    public class UserStore: MonoSingleton<UserStore>
    {
        public string APIUrl;
        public string Username;
        public string Password;
    }
}
