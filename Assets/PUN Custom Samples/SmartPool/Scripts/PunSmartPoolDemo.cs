using UnityEngine;
using System.Collections;

using ExitGames.Client.Photon;

/// <summary>
/// Pun smart pool demo. Simple script to instantiate the player. Note that this script doesn't change whether you use a pool manager or not.
/// Check out PunSmartPoolBridge.cs to see how pooling is enabled.
/// </summary>
public class PunSmartPoolDemo: Photon.PunBehaviour {
	
	public override void OnJoinedRoom ()
	{
		PhotonNetwork.Instantiate("SmartPoolPlayer", new Vector3(0f,0f,0f), Quaternion.identity, 0);
	}

}
