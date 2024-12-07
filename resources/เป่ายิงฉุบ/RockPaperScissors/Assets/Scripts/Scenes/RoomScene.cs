using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RockPaperScissors
{
    public class RoomScene : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI player1TMP;
        [SerializeField]
        TextMeshProUGUI player2TMP;

        Main Main => Main.Instance; 
        RoomStore RoomStore => RoomStore.Instance;
        UserStore UserStore => UserStore.Instance;

        void Start()
        {
            RoomStore.RoomUpdated += UpdateRoom;
            UpdateRoom();
        }

        void OnDestroy()
        {
            RoomStore.RoomUpdated -= UpdateRoom;
        }
        public void LeaveRoom()
        {
            RoomStore.LeaveRoom(RoomStore.CurrentRoom.id).Forget();
            Main.UnloadScene("Room").Forget();
            Main.LoadSceneIfNotPresent("RoomSelect").Forget();
        }

        private void UpdateRoom()
        {
            if (RoomStore.CurrentRoom != null)
            {
                var user1 = UserStore.UserById.GetValueOrDefault(RoomStore.CurrentRoom.player1, null);
                var user2 = UserStore.UserById.GetValueOrDefault(RoomStore.CurrentRoom.Player2, null);

                if (user1 != null)
                {
                    player1TMP.text = user1.username;
                }
                if (user2 != null)
                {
                    player2TMP.text = user2.username;
                }

                if (user1 != null && user2 != null)
                {
                    Main.isLoading = true;
                    Invoke("LoadGame", 3f);
                }
            }
        }

        private void LoadGame()
        {
            Main.isLoading = false;
            Main.UnloadScene("Room").Forget();
            Main.LoadSceneIfNotPresent("Game").Forget();
        }
    }
}
