using System.Collections.Generic;

namespace RockPaperScissors.Models
{
    public class UserModel
    {
        public string id { get; set; }
        public string username { get; set; }
    }

    public class UserListResponse
    {
        public List<UserModel> users { get; set; }
    }
}
