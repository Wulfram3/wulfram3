// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerNameInputField.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Let the player input his name to be saved as the network player Name, viewed by alls players above each  when in the same room. 
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using Assets.Plugins.webgljs;
using Assets.InternalApis;
using Assets.InternalApis.Interfaces;

namespace ExitGames.Demos.DemoAnimator
{
	/// <summary>
	/// Player name input field. Let the user input his name, will appear above the player in the game.
	/// </summary>
	[RequireComponent(typeof(InputField))]
	public class PlayerNameInputField : MonoBehaviour
	{
		#region Private Variables

		// Store the PlayerPref Key to avoid typos
		static string playerNamePrefKey = "PlayerName";

		#endregion

		#region MonoBehaviour CallBacks
		
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start () {
            var storage = DepenencyInjector.Resolve<IInternalStorage>();

            string defaultName = "";
			InputField _inputField = this.GetComponent<InputField>();
            Debug.Log("defaultName:" + defaultName);

            if (_inputField!=null)
			{
                var userString = storage.GetValue("Name");
                if (userString != "null")
                {
                    // Auth'ed User
                    defaultName = userString;
                    Debug.Log("defaultName:" + defaultName);
                }
                else
                {
                    if (PlayerPrefs.HasKey(playerNamePrefKey))
                    {
                        defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                        Debug.Log("defaultName:" + defaultName);
                    }
                    else
                    {
                        var rnd = new System.Random();
                        defaultName = "GuestUser#" + rnd.Next(1, 9000);
                        Debug.Log("defaultName:" + defaultName);
                    }
                }
            }

            Debug.Log("defaultName:" + defaultName);
            _inputField.text = defaultName;
            PhotonNetwork.playerName =	defaultName;
		}

		#endregion
		
		#region Public Methods

		/// <summary>
		/// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
		/// </summary>
		/// <param name="value">The name of the Player</param>
		public void SetPlayerName(string value)
		{
			// #Important
			PhotonNetwork.playerName = value + " "; // force a trailing space string in case value is an empty string, else playerName would not be updated.

			PlayerPrefs.SetString(playerNamePrefKey,value);
		}
		
		#endregion
	}
}
