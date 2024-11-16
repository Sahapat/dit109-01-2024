using UnityEngine;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;

namespace RockPaperScissors
{
    public class HTTPService
    {
        private readonly string BaseURL;
        private string Username;
        private string Password;

        private HTTPService() { }
        public HTTPService(string baseUrl)
        {
            BaseURL = baseUrl;
        }

        public void SetCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        private string GetCredentials()
        {
            string credentials = $"{Username}:{Password}";
            string encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            return encodedCredentials;
        }

        public async UniTask<string> GetAsync(string parameters)
        {
            var requestUrl = $"{BaseURL}/{parameters}";
            var requester = UnityWebRequest.Get(requestUrl);
            requester.SetRequestHeader("Authorization", "Basic " + GetCredentials());
            var response = (await requester.SendWebRequest()).downloadHandler.text;
            Debug.Log(response);
            return response;
        }

        public async UniTask<string> PostAsync(string parameters, object body)
        {
            var requestUrl = $"{BaseURL}/{parameters}";
            var postDataJson = JsonUtility.ToJson(body);
            var requester = UnityWebRequest.Post(requestUrl, postDataJson, "application/json");
            requester.SetRequestHeader("Authorization", "Basic " + GetCredentials());
            var response = (await requester.SendWebRequest()).downloadHandler.text;
            return response;
        }
    }
}
