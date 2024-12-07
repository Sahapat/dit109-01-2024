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
            GameStore.SendChoice(RoomStore.CurrentRoom.ID, choice).Forget();
        }

        private async UniTask CheckWinner()
        {
            var result = await GameStore.CheckWinner(RoomStore.CurrentRoom.ID);
            _FinishText.text = result;
            _Player1ChoiceText.text = RoomStore.CurrentRoom.P1Choice;
            _Player2ChoiceText.text = RoomStore.CurrentRoom.P2Choice;
        }

        private void UpdateRoomDisplay()
        {
            if (RoomStore.CurrentRoom.IsGameFinish)
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
