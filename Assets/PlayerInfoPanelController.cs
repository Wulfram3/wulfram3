using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Wulfram3 {
    public class PlayerInfoPanelController : Photon.PunBehaviour {
        private GameObject target;
        private Vector3 pos;
        private GameManager gameManager;

        public Text playerNameText;

        // Use this for initialization
        void Start() {
            Canvas canvas = FindObjectOfType<Canvas>();
            transform.SetParent(canvas.transform);
        }

        // Update is called once per frame
        void LateUpdate() {
            if (target != null && target.GetComponentInChildren<Renderer>().isVisible) {
                playerNameText.gameObject.SetActive(true);
                pos = Camera.main.WorldToScreenPoint(target.transform.position);
                pos.z = 0;
                RectTransform rectTransform = GetComponent<RectTransform>();
                pos.y += 50;

                string playerName = target.GetComponent<PhotonView>().owner.NickName;
                string hitpoints = target.GetComponent<HitPointsManager>().health + "/" + target.GetComponent<HitPointsManager>().maxHealth;
                playerNameText.text = playerName + " " + hitpoints;
                if (target.GetComponent<PhotonView>().owner.IsMasterClient) {
                    playerNameText.color = Color.yellow;
                } else {
                    playerNameText.color = Color.white;
                }        

                rectTransform.SetPositionAndRotation(pos, rectTransform.rotation);
            } else {
                playerNameText.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update() {
 
        }

        public void SetTarget(GameObject target) {
            this.target = target;
        }

    }
}