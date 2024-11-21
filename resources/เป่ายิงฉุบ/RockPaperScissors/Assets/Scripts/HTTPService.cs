using UnityEngine;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;

namespace RockPaperScissors
{
    public class HTTPService
    {
        private string _ServerURL;
        private string _Username;
        private string _Password;

        public HTTPService(string serverUrl)
        {
            _ServerURL = serverUrl;
        }
        private HTTPService() {}

        public HTTPService SetCredential(string username, string password)
        {
            _Username = username;
            _Password = password;
            return this;
        }

        private string GetCredential()
        {
            string credential = $"{_Username}:{_Password}";
            string encodedCredential = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
            return encodedCredential;
        }

        public async UniTask<string> GetAsync(string parameters)
        {
            var requestUrl = $"{_ServerURL}{parameters}";
            var requester = UnityWebRequest.Get(requestUrl);
            requester.SetRequestHeader("Authorization", $"Basic {GetCredential()}");
            var response = (await requester.SendWebRequest()).downloadHandler.text;
            return response;
        }

        public async UniTask<string> PostAsync(string parameters, object body)
        {
            var requestUrl = $"{_ServerURL}{parameters}";
            var dataJson = JsonUtility.ToJson(body);
            var requester = UnityWebRequest.Post(requestUrl, dataJson, "application/json");
            requester.SetRequestHeader("Authorization", $"Basic {GetCredential()}");
            var response = (await requester.SendWebRequest()).downloadHandler.text;
            return response;
        }
    }
}
