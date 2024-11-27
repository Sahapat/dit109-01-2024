using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using InspireTale.Utils;
using System.Collections.Generic;

namespace RockPaperScissors
{
    public class Main : MonoSingleton<Main>
    {
        [SerializeField]
        List<string> CurrentScenes = new();

        public string APIUrl;
        public string APIKey;
        public bool isLoading;

        void Start()
        {
           LoadSceneIfNotPresent("Login").Forget(); 
        }

        public async UniTask UnloadAllScenes()
        {
            await UniTask.WhenAll(CurrentScenes.Select(async sceneName =>
            {
                await SceneManager.UnloadSceneAsync(sceneName);
            }));
        }

        public async UniTask UnloadScene(string sceneName)
        {
            var index = CurrentScenes.IndexOf(sceneName);
            await SceneManager.UnloadSceneAsync(CurrentScenes[index]);
            CurrentScenes.RemoveAt(index);
        }

        public async UniTask LoadSceneIfNotPresent(string sceneName)
        {
            if (!IsSceneLoaded(sceneName))
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
                CurrentScenes.Add(sceneName);
            }
        }

        private bool IsSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
