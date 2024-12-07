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
            var user = UserStore.UserById.GetValueOrDefault(room.Player1, null);
            _RoomNameText.text = user == null ? room.Player1 : user.name;
        }

        public void HandleClick()
        {
            HandleClickAsync().Forget();
        }
        private async UniTask HandleClickAsync()
        {
            await RoomStore.JoinRoomById(_Room.ID);
            GameStore.RoomId = _Room.ID;
            Main.UnloadScene("RoomSelect").Forget();
            Main.LoadSceneIfNotPresent("Room").Forget();
        }
    }
}
