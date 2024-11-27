using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using InspireTale.Utils;
using Newtonsoft.Json;
using RockPaperScissors.Models;

namespace RockPaperScissors
{
    public class UserStore: MonoSingleton<UserStore>
    {
        public string Username;
        public string Password;

        public List<UserModel> Users;
        public Dictionary<string, UserModel> UserById;

        Main Main => Main.Instance;
        HTTPService _HTTPService
        {
            get
            {
                var service = new HTTPService(Main.APIUrl, Main.APIKey)
                .SetCredential(Username, Password);
                return service;
            }
        }
        public async UniTask<List<UserModel>> GetAllUsers()
        {
            var json = await _HTTPService.GetAsync("users");
            var users = JsonConvert.DeserializeObject<List<UserModel>>(json);
            Users = users;
            UserById = users.ToDictionary(v => v.ID);
            return users;
        }
    }
}
