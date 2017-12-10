using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class PlayerInfoController : Photon.PunBehaviour {

        private GameObject playerInfoPanel = null;

        // Use this for initialization
        void Start() {
            //do not show panel for my player, only for others
            if (!photonView.isMine) {
                GameManager gameManager = FindObjectOfType<GameManager>();
                playerInfoPanel = Instantiate<GameObject>(gameManager.playerInfoPanelPrefab);
                playerInfoPanel.GetComponent<PlayerInfoPanelController>().SetTarget(gameObject);
            }
        }

        // Update is called once per frame
        void Update() {

        }

        void OnDestroy() {
            if (playerInfoPanel != null) {
                Destroy(playerInfoPanel);
            }
        }
    }
}

