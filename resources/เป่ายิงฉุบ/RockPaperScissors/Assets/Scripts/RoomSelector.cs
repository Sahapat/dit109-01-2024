using RockPaperScissors.Models;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace RockPaperScissors
{
    public class RoomSelector : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _RoomNameText;

        private RoomModel _Room;
        GameStore GameStore => GameStore.Instance;
        RoomStore RoomStore => RoomStore.Instance;
        UserStore UserStore => UserStore.Instance;
        Main Main => Main.Instance;
        public void Setup(RoomModel room)
        {
            _Room = room;
            var user = UserStore.UserById.GetValueOrDefault(room.player1, null);
            _RoomNameText.text = user == null ? room.player1 : user.username;
        }

        public void HandleClick()
        {
            HandleClickAsync().Forget();
        }
        private async UniTask HandleClickAsync()
        {
            await RoomStore.JoinRoomById(_Room.id);
            GameStore.RoomId = _Room.id;
            Main.UnloadScene("RoomSelect").Forget();
            Main.LoadSceneIfNotPresent("Room").Forget();
        }
    }
}
