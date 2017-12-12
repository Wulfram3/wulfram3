//using Assets.Plugins.webgljs;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Com.Wulfram3
{
    public class DiscordApi
    {
        private const string channelUrl = "https://discordapp.com/api/webhooks/389264790230532107/LgvTNdOLb28JQmtTpK1yBzam-CMAnEhDqLkmXT4CqAyP-8id8ydWisx2yz8Ga6fQ5wX2";
        private const string joinMessage = "{0} has started playing Wulfram 3!";
        private const string leftMessage = "{0} has left Wulfram 3!";
        private string player;

       /* public DiscordApi()
        {
            var userString = WebLocalStorage.GetValue("test");
            Debug.Log("User from Local Storage: " + userString);


            WebLocalStorage.SetValue("test", @"{""username:""""Knight""}");
            userString = WebLocalStorage.GetValue("test");
            Debug.Log("User from Local Storage After set: " + userString);

        }*/

        public IEnumerator PlayerJoined(string playerName)
        {
            var greetdiscord = string.Format(joinMessage, playerName);
            var postdiscord = "{ \"content\": \"" + greetdiscord + "\" } ";

            yield return Post(channelUrl, postdiscord);
        }

        public IEnumerator PlayerLeft(string playerName)
        {
            var greetdiscord = string.Format(leftMessage, playerName);
            var postdiscord = "{ \"content\": \"" + greetdiscord + "\" } ";

            yield return Post(channelUrl, postdiscord);
        }


        IEnumerator Post(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.Send();

            Debug.Log("Status Code: " + request.responseCode);
        }
    }
}