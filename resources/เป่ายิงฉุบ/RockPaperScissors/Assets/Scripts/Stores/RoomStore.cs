using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InspireTale.Utils;
using RockPaperScissors.Models;
using UnityEngine;

namespace RockPaperScissors
{
    public class RoomStore: MonoSingleton<RoomStore>
    {
        public RoomModel CurrentRoom;
        public List<RoomModel> RoomList;

        public delegate void Notify();
        public event Notify RoomListUpdated;
        public event Notify RoomUpdated;

        Main Main => Main.Instance;
        UserStore UserStore => UserStore.Instance;
        HTTPService _HTTPService
        {
            get
            {
                var service = new HTTPService(Main.APIUrl, Main.APIKey)
                .SetCredential(UserStore.Username, UserStore.Password);
                return service;
            }
        }

        public async UniTask<List<RoomModel>> FetchRoomList()
        {
            var json = await _HTTPService.GetAsync("rooms");
            var roomResponse = JsonUtility.FromJson<RoomListResponse>(json);
            var roomList = roomResponse.rooms;
            RoomList = roomList;
            RoomListUpdated?.Invoke();
            return roomList;
        }

        public async UniTask<RoomModel> GetRoomById(string roomId)
        {
            var json = await _HTTPService.GetAsync($"room/{roomId}");
            var room = JsonUtility.FromJson<RoomModel>(json);
            CurrentRoom = room;
            RoomUpdated?.Invoke();
            return  room;
        }
        public async UniTask<RoomModel> CreateRoom()
        {
            Main.isLoading = true;
            var json = await _HTTPService.PostAsync($"room", new());
            var room = JsonUtility.FromJson<RoomModel>(json);
            CurrentRoom = room;
            RoomUpdated?.Invoke();
            Main.isLoading = false;
            return room;
        }

        public async UniTask<RoomModel> JoinRoomById(string roomId)
        {
            Main.isLoading = true;
            await _HTTPService.PostAsync($"room/join/{roomId}", new());
            var room = await GetRoomById(roomId);
            Main.isLoading = false;
            return room;
        }

        public async UniTask LeaveRoom(string roomId)
        {
            await _HTTPService.PostAsync($"room/leave/{roomId}", new());
            await FetchRoomList();
            RoomUpdated?.Invoke();
        }
    }
}
