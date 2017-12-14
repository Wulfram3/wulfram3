//using Assets.Plugins.webgljs;
using Assets.InternalApis.Interfaces;
using Assets.Plugins.webgljs;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.InternalApis.Implementations
{
    /// <summary>
    /// This will access the users Discord account, getting data about a user (Username, email, ect.)
    /// </summary>
    public class DiscordApi : IDiscordApi
    {
        private const string discordApiUrl = "https://discordapp.com/api/v6";
        private const string channelUrl = "https://discordapp.com/api/webhooks/389264790230532107/LgvTNdOLb28JQmtTpK1yBzam-CMAnEhDqLkmXT4CqAyP-8id8ydWisx2yz8Ga6fQ5wX2";
        private const string joinMessage = "{0} has started playing Wulfram 3!";
        private const string leftMessage = "{0} has left Wulfram 3!";

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

        private IEnumerator Post(string url, string bodyJsonString)
        {
            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.Send();

                Debug.Log("Status Code: " + request.responseCode);
            }
        }

        private IEnumerator Get(string url, string bodyJsonString)
        {
            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.Send();

                Debug.Log("Status Code: " + request.responseCode);
            }
        }

    }
}