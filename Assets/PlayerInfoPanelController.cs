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

        


        //public Color red = new Color(249F/255F, 32F/255F, 57F/255F);
        //public Color blue = new Color(31F/255F, 118F/255F, 204F/255F);

        // Use this for initialization
        void Start() {
            Canvas canvas = FindObjectOfType<Canvas>();
            transform.SetParent(canvas.transform);
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void LateUpdate() {
            if (target != null && target.GetComponentInChildren<MeshRenderer>().isVisible && Camera.main != null) {
                playerNameText.gameObject.SetActive(true);
                pos = Camera.main.WorldToScreenPoint(target.transform.position);
                pos.z = 0;
                RectTransform rectTransform = GetComponent<RectTransform>();
                pos.y += 50;

                string playerName = target.GetComponent<PhotonView>().owner.NickName;
                string hitpoints = target.GetComponent<HitPointsManager>().health + "/" + target.GetComponent<HitPointsManager>().maxHealth;

                //string masterClient = "";
                //string moddevTag = "";
                //string username = "";
                //if (target.GetComponent<PhotonView>().owner.IsMasterClient) {
                //    masterClient = "<color=magenta>*</color>";
                //}

                //if (playerName.Contains("[MOD]"))
                //{
                //    moddevTag = "<color=yellow>[MOD]</color>";
                //}

                //if (playerName.Contains("[DEV]"))
                //{
                //    moddevTag = "<color=orange>[DEV]</color>";
                //}


                var name = gameManager.GetColoredPlayerName(playerName, target.GetComponent<PhotonView>().owner.IsMasterClient, true, target.GetComponent<Unit>().unitTeam);
                playerNameText.text = name;

                //if (target.GetComponent<PhotonView>().owner.GetTeam().Equals(PunTeams.Team.red)) {
                //    //red
                //    playerNameText.color = gameManager.redcolor.color;
                //} else {
                //    //blue
                //    playerNameText.color = gameManager.bluecolor.color;
                //}
                //add mod check here




                /* if (target.GetComponent<PhotonView>().owner.IsMasterClient) {
                     playerNameText.color = Color.yellow;
                 }*/




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