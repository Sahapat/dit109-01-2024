using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using InspireTale.Utils;
using RockPaperScissors.Models;
using UnityEngine;

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
            var userResponse = JsonUtility.FromJson<UserListResponse>(json);
            var users = userResponse.users;
            Users = users;
            UserById = users.ToDictionary(v => v.id);
            return users;
        }
    }
}
