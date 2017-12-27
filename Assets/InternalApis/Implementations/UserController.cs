using Assets.InternalApis.Classes;
using Assets.InternalApis.Interfaces;
using Newtonsoft.Json;
using socket.io;
using System;
using UnityEngine;

namespace Assets.InternalApis.Implementations
{
    public class UserController : IUserController
    {

        private Socket socketServer;
        public UserController()
        {
            player = new WulframPlayer();
            //player.Username = GetUsername();

            Debug.Log("UserController constructor:" + player.userName);
            SetupSocketConnection();
        }

        private WulframPlayer player;

        public event Action<WulframPlayer, string> LoginCompleted;
        public event Action<string> RegisterUserCompleted;

        public WulframPlayer GetWulframPlayerData()
        {
            if(string.IsNullOrEmpty(player.userName))
            {
                player.userName = GetUsername();
            }
            return player;
        }

        public void LoginUser(string username, string password)
        {
            socketServer.EmitJson("login", JsonConvert.SerializeObject(new { username = username, password  = password }));
        }

        public void RegisterUser(string username, string password, string email)
        {
            socketServer.EmitJson("registerNewUser", JsonConvert.SerializeObject(new { userName = username, password = password, email = email }));
        }

        public void UpdateUserData()
        {

        }

        private string GetUsername()
        {
            PlayerPrefs.DeleteAll();
            string defaultName = "";
            Debug.Log("defaultName:" + defaultName);

            var userString = this.player.userName;
            if (userString != "null")
            {
                // Auth'ed User
                defaultName = userString;
                Debug.Log("defaultName:" + defaultName);
            }
            else
            {
                if (PlayerPrefs.HasKey("PlayerName"))
                {
                    defaultName = PlayerPrefs.GetString("PlayerName");
                    Debug.Log("defaultName:" + defaultName);
                }
                else
                {
                    defaultName = "GuestUser#" + new System.Random().Next(1, 9000);
                    Debug.Log("defaultName:" + defaultName);
                }
            }

            Debug.Log("defaultName:" + defaultName);
            PhotonNetwork.playerName = defaultName;
            return defaultName;
        }

        


        private void SetupSocketConnection()
        {
            //var serverUrl = "http://localhost:8080";
            //var serverUrl = "http://wulfram-player-node.herokuapp.com/";
            var serverUrl = "http://wulfram.com:1337/";
            socketServer = Socket.Connect(serverUrl);

            socketServer.On(SystemEvents.connect, () => {
                Debug.Log("Hello, Socket.io~");
            });

            socketServer.On("handshake", (string data) => {
                Debug.Log(data);
            });

            socketServer.On("loginCompleted", (string data) => {
                Debug.Log("loginCompleted:" + data);
                this.player = Newtonsoft.Json.JsonConvert.DeserializeObject<WulframPlayer>(data);
                GetUsername();
                LoginCompleted.Invoke(player, "Login Complete");
            });

            socketServer.On("loginFailed", (string data) => {
                LoginCompleted.Invoke(null, "Login Failed");
            });

            socketServer.On("registerComplete", (string data) => {
                RegisterUserCompleted.Invoke(data);
            });

            socketServer.On("registerFailed", (string data) => {
                RegisterUserCompleted.Invoke(data);
            });

            socketServer.On(SystemEvents.reconnect, (int reconnectAttempt) => {
                Debug.Log("Hello, Again! " + reconnectAttempt);
            });

            socketServer.On(SystemEvents.disconnect, () => {
                Debug.Log("Bye~");
            });
        }
    }
}