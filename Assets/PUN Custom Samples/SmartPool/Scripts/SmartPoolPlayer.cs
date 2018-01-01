using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ExitGames.Client.Photon;

/// <summary>
/// Smart pool player. Simple Player control to move cube around with arrow keys.
/// It also demonstrate initialization dos and donts.
/// </summary>
public class SmartPoolPlayer : Photon.PunBehaviour {

	// not good as it happens too early when instance is reused.
	void OnEnable () {

		Debug.Log("SmartPoolPlayer Instance OnEnable: ownerID:"+this.photonView.ownerId+" isMine:"+this.photonView.isMine+" viewID:"+this.photonView.viewID);
	}

	// not good as it happens only the first time the instance is returned by the pool
	void Start () {

		Debug.Log("SmartPoolPlayer Instance Start: ownerID:"+this.photonView.ownerId+" isMine:"+this.photonView.isMine+" viewID:"+this.photonView.viewID);
	}

	// right time to get early information
	public override void OnPhotonInstantiate (PhotonMessageInfo info)
	{

		Debug.Log("SmartPoolPlayer Instance OnPhotonInstantiate: ownerID:"+this.photonView.ownerId+" isMine:"+this.photonView.isMine+" viewID:"+this.photonView.viewID);
	}
	
	void OnDisable () {
		Debug.Log("SmartPoolPlayer Instance OnDisable\t: ownerID:"+this.photonView.ownerId+" isMine:"+this.photonView.isMine+" viewID:"+this.photonView.viewID);
	}


	// quick little user control to move the cube around
	#region Controls
	public float Speed = 3f;
	float range = 4;
	Vector3 _pos;
	
	void Update () {

		if (this.photonView.isMine)
		{
			_pos = this.transform.position;
			_pos.x = Mathf.Clamp( _pos.x+Input.GetAxis("Horizontal")*Speed*Time.deltaTime,-range,range);
			_pos.y = Mathf.Clamp( _pos.y+Input.GetAxis("Vertical")*Speed*Time.deltaTime,-range,range);
			this.transform.position = _pos;

		}
		
	}

	#endregion

}
