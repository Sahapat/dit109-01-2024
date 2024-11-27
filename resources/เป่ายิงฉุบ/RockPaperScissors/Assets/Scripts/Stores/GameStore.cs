using Cysharp.Threading.Tasks;
using InspireTale.Utils;

namespace RockPaperScissors
{
    public class GameStore: MonoSingleton<GameStore>
    {
        public string RoomId;
        Main Main => Main.Instance;
        UserStore UserStore => UserStore.Instance;
        HTTPService _HTTPService
        {
            get
            {
                var service = new HTTPService(Main.APIUrl, Main.APIKey)
                .SetCredential(UserStore.Username, UserStore.Password);
                return service;
            }
        }

        public async UniTask SendChoice(string roomId, string choice)
        {
            Main.isLoading = true;
            var choiceModel = new ChoiceModel { choice = choice };
            await _HTTPService.PostAsync($"room/send_choice/{roomId}", choiceModel);
            Main.isLoading = false;
        }

        public async UniTask<string> CheckWinner(string roomId)
        {
            Main.isLoading = true;
            var result = await _HTTPService.PostAsync($"room/check_winner/{roomId}", new());
            Main.isLoading = false;
            return result;
        }
    }
}
