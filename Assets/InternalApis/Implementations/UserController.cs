using Assets.InternalApis.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Implementations
{
    public class UserController : IUserController
    {
        public string GetUsername()
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
                    var rnd = new System.Random();
                    defaultName = "GuestUser#" + rnd.Next(1, 9000);
                    Debug.Log("defaultName:" + defaultName);
                }
            }

            Debug.Log("defaultName:" + defaultName);
            return defaultName;
        }
    }
}