using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro.SpriteAssetUtilities;
using TMPro;
using Cysharp.Threading.Tasks;
using EasyUI.Popup;

namespace RockPaperScissors
{
    public class LoginMenu : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField _ServerUrl;
        [SerializeField]
        TMP_InputField _Username;
        [SerializeField]
        TMP_InputField _Password;

        UserStore UserStore => UserStore.Instance;

        public void OnServerChanged()
        {
            UserStore.APIUrl = _ServerUrl.text;
        }
        public void OnUsernameChanged()
        {
            UserStore.Username = _Username.text;
        }
        public void OnPasswordChanged()
        {
            UserStore.Password = _Password.text;
        }

        public void Login()
        {
            DoLogin().Forget();
        }

        private async UniTask DoLogin()
        {
            var service = new HTTPService(UserStore.APIUrl);
            service.SetCredential(UserStore.Username, UserStore.Password);
            try
            {
                var response = await service.GetAsync("/user/all");
            }
            catch (Exception ex)
            {
                Popup.Show("<color=red>Request ERROR</color>", ex.Message);
            }
        }
    }
}
