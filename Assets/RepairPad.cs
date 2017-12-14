using Com.Wulfram3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPad : MonoBehaviour {

    private GameManager gameManager;

	void Awake ()
    {
        gameManager = FindObjectOfType<GameManager>();
	}


    void OnMouseDown()
    {
        PlayerMovementManager player = PlayerMovementManager.LocalPlayerInstance.GetComponent<PlayerMovementManager>();
        if (player.isDead)
        {
            gameManager.normalCamera.enabled = true;
            gameManager.overheadCamera.enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            player.photonView.RPC("SetPosAndRotation", PhotonTargets.All, gameObject.transform.position + new Vector3(0, 5, 0), Quaternion.identity);

            if(PhotonNetwork.isMasterClient)
            {
                HitPointsManager hitpointsManager = player.GetComponent<HitPointsManager>();
                hitpointsManager.SetHealth(hitpointsManager.maxHealth);
            }
        }
    }
}
