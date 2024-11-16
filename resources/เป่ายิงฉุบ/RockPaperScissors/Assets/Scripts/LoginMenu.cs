using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro.SpriteAssetUtilities;
using TMPro;
using Cysharp.Threading.Tasks;

namespace RockPaperScissors
{
    public class LoginMenu : MonoBehaviour
    {
        [SerializeField]
        public TMP_InputField _apiInput;
        [SerializeField]
        public TMP_InputField _usernameInput;
        [SerializeField]
        public TMP_InputField _passwordInput;
        [SerializeField]
        public TextMeshProUGUI _resultText;

        private UserStore UserStore => UserStore.Instance;
        
        public void HandleApiUrlChanged()
        {
            UserStore.APIUrl = _apiInput.text;
        }

        public void HandleUsernameChanged()
        {
            UserStore.Username = _usernameInput.text;
        }
        
        public void HandlePasswordChanged()
        {
            UserStore.Password = _passwordInput.text;
        }

        public void GetAllUser()
        {
            DoGetAllUser().Forget();
        }

        private async UniTask DoGetAllUser()
        {
            var httpService = new HTTPService(UserStore.APIUrl);
            var result = await httpService.GetAsync("user/all");
            _resultText.text = result;
        }

        public void GetRoom()
        {
            DoGetRoom().Forget();
        }

        private async UniTask DoGetRoom()
        {
            var httpService = new HTTPService(UserStore.APIUrl);
            var result = await httpService.GetAsync("room_state");
            _resultText.text = result;
        }
    }
}
