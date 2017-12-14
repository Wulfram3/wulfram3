using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Interfaces
{
    public interface IDiscordApi
    {
        IEnumerator PlayerJoined(string playerName);

        IEnumerator PlayerLeft(string playerName);
    }
}