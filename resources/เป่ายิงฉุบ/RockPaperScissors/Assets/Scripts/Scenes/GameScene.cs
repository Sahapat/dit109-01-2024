using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace RockPaperScissors
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField]
        GameObject _GamePlayObj;
        [SerializeField]
        GameObject _FinishObj;
        [SerializeField]
        TextMeshProUGUI _Player1ChoiceText;
        [SerializeField]
        TextMeshProUGUI _Player2ChoiceText;
        [SerializeField]
        TextMeshProUGUI _FinishText;

        RoomStore RoomStore => RoomStore.Instance;
        GameStore GameStore => GameStore.Instance;
        Main Main => Main.Instance;

        void Start()
        {
            UpdateRoomDisplay();
            RoomStore.RoomUpdated += UpdateRoomDisplay;
        }

        void Destroy()
        {
            RoomStore.RoomUpdated -= UpdateRoomDisplay;
        }

        public void SendChoice(string choice)
        {
            GameStore.SendChoice(RoomStore.CurrentRoom.id, choice).Forget();
        }

        private async UniTask CheckWinner()
        {
            var result = await GameStore.CheckWinner(RoomStore.CurrentRoom.id);
            _FinishText.text = result;
            _Player1ChoiceText.text = RoomStore.CurrentRoom.p1_choice;
            _Player2ChoiceText.text = RoomStore.CurrentRoom.p2_choice;
        }

        private void UpdateRoomDisplay()
        {
            if (RoomStore.CurrentRoom.is_game_finish)
            {
                _GamePlayObj.SetActive(false);
                _FinishObj.SetActive(true);
                CheckWinner().Forget();
            }
            else
            {
                _GamePlayObj.SetActive(true);
                _FinishObj.SetActive(false);
            }
        }
    }
}
