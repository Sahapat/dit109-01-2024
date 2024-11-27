using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace RockPaperScissors
{
    public class LoginScene : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField _ServerUrl;
        [SerializeField]
        TMP_InputField _APIKey;
        [SerializeField]
        TMP_InputField _Username;
        [SerializeField]
        TMP_InputField _Password;

        UserStore UserStore => UserStore.Instance;
        Main Main => Main.Instance;

        public void OnServerChanged()
        {
            Main.APIUrl = _ServerUrl.text;
        }
        public void OnUsernameChanged()
        {
            UserStore.Username = _Username.text;
        }
        public void OnPasswordChanged()
        {
            UserStore.Password = _Password.text;
        }
        public void OnAPIKeyChanged()
        {
            Main.APIKey = _APIKey.text;
        }

        public void Login()
        {
            LoginAsync().Forget();
        }

        public async UniTask LoginAsync()
        {
            await UserStore.GetAllUsers();
            Main.UnloadScene("Login").Forget();
            Main.LoadSceneIfNotPresent("Overlay").Forget();
            Main.LoadSceneIfNotPresent("RoomSelect").Forget();
        }
    }
}
