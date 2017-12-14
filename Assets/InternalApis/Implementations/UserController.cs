using Assets.InternalApis.Classes;
using Assets.InternalApis.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Implementations
{
    public class UserController : IUserController
    {
        public UserController()
        {
            player = new WulframPlayer();
            player.Username = GetUsername();

            Debug.Log("UserController constructor:" + player.Username);
        }

        private WulframPlayer player;

        

        public WulframPlayer GetWulframPlayerData()
        {
            if(string.IsNullOrEmpty(player.Username))
            {
                player.Username = GetUsername();
            }
            return player;
        }

        private string GetUsername()
        {
            var storage = DepenencyInjector.Resolve<IInternalStorage>();
            PlayerPrefs.DeleteAll();
            string defaultName = "";
            Debug.Log("defaultName:" + defaultName);

            var userString = storage.GetValue("Name");
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
            return defaultName;
        }

        public void UpdateUserData()
        {
           
        }
    }
}