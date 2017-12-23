using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class CargoManager : Photon.PunBehaviour {

        public string pickedUpCargo = "";

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (photonView.isMine) {     
                if (Input.GetKeyDown("q")) {
                    GetGameManager().PickUpCargo(this);
                }

                if (Input.GetKeyDown("z")) {
                    GetGameManager().DropCargo(this);
                }

                if (pickedUpCargo != "") {
                    //TODO: show animation of cargo in player HUD
                }
            }

        }

        [PunRPC]
        public void SetPickedUpCargo(string cargo) {
            pickedUpCargo = cargo;
            if (photonView.isMine) {
                if (cargo != "") {
                    //TODO: play pickup sound
                } else {
                    //TODO: play drop sound
                }
            }
        }

        private GameManager GetGameManager() {
            return FindObjectOfType<GameManager>();
        }
    }
}
