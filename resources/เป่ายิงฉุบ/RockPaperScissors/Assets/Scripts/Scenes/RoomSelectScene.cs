using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RockPaperScissors
{
    public class RoomSelectScene : MonoBehaviour
    {
        [SerializeField]
        Transform _ScrollViewContent;
        [SerializeField]
        GameObject _RoomSeletorPrefab;

        List<RoomSelector> PrefabPool = new();

        Main Main => Main.Instance;
        RoomStore RoomStore => RoomStore.Instance;
        void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                var obj = Object.Instantiate(_RoomSeletorPrefab, _ScrollViewContent);
                var script = obj.GetComponent<RoomSelector>();
                obj.SetActive(false);
                PrefabPool.Add(script);
            }
            RoomStore.RoomListUpdated += UpdateRoomList;
            Refresh();
        }

        void OnDestroy()
        {
            RoomStore.RoomListUpdated -= UpdateRoomList;
        }

        public void Refresh()
        {

            RoomStore.FetchRoomList().Forget();
        }

        public void CreateRoom()
        {
            CreateRoomAsync().Forget();
        }
        private async UniTask CreateRoomAsync()
        {
            await RoomStore.CreateRoom();
            Main.UnloadScene("RoomSelect").Forget();
            Main.LoadSceneIfNotPresent("Room").Forget();
        }

        private void UpdateRoomList()
        {
            var diff = RoomStore.RoomList.Count - PrefabPool.Count;
            for (int i = 0; i < diff; i++)
            {
                var obj = Object.Instantiate(_RoomSeletorPrefab, _ScrollViewContent);
                var script = obj.GetComponent<RoomSelector>();
                obj.SetActive(false);
                PrefabPool.Add(script);
            }

            for (int i = 0; i < PrefabPool.Count; i++)
            {
                var roomObj = PrefabPool[i];
                if (i < RoomStore.RoomList.Count)
                {
                    var room = RoomStore.RoomList[i];
                    roomObj.gameObject.SetActive(true);
                    roomObj.Setup(room);
                }
                else
                {
                    roomObj.gameObject.SetActive(false);
                }
            }
        }
    }
}
