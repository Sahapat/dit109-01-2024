using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using EasyUI.Popup;

namespace RockPaperScissors
{
    public class HTTPService
    {
        private string _ServerURL;
        private string _APIKey;
        private string _Username;
        private string _Password;

        public HTTPService(string serverUrl, string apiKey)
        {
            _ServerURL = serverUrl;
            _APIKey = apiKey;
        }
        private HTTPService() { }

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
            try
            {
                var uri = new Uri($"{_ServerURL}/{parameters}");
                var requester = UnityWebRequest.Get($"{_ServerURL}/{parameters}");
                requester.SetRequestHeader("Authorization", $"Basic {GetCredential()}");
                requester.SetRequestHeader("X-API-KEY", _APIKey);
                requester.SetRequestHeader("Accept", "application/json");
                requester.SetRequestHeader("ngrok-skip-browser-warning", "true");
                Debug.Log($"Requesting to: {uri.ToString()}");
                var response = (await requester.SendWebRequest()).downloadHandler.text;
                return response;
            }
            catch (Exception ex)
            {
                Popup.Show("<color=red>Request ERROR</color>", ex.Message);
                throw;
            }
        }

        public async UniTask<string> PostAsync(string parameters, object body)
        {
            try
            {
                var uri = new Uri($"{_ServerURL}/{parameters}");
                var dataJson = JsonConvert.SerializeObject(body);
                var requester = UnityWebRequest.Post(uri.ToString(), dataJson, "application/json");
                requester.SetRequestHeader("Authorization", $"Basic {GetCredential()}");
                requester.SetRequestHeader("X-API-KEY", _APIKey);
                requester.SetRequestHeader("Accept", "application/json");
                requester.SetRequestHeader("ngrok-skip-browser-warning", "true");
                Debug.Log($"Requesting to: {uri.ToString()}");
                var response = (await requester.SendWebRequest()).downloadHandler.text;
                return response;
            }
            catch (Exception ex)
            {
                Popup.Show("<color=red>Request ERROR</color>", ex.Message);
                throw;
            }
        }
    }
}
