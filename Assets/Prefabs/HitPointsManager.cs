using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class HitPointsManager : Photon.PunBehaviour {

        public int initialHealth = 100;
        public int maxHealth = 100;

        private int health;

        public void TakeDamage(int amount) {
            if (PhotonNetwork.isMasterClient) {
                int newHealth = Mathf.Clamp(health - amount, 0, maxHealth);
                photonView.RPC("UpdateHealth", PhotonTargets.All, newHealth);
            }
        }

        [PunRPC]
        public void UpdateHealth(int newHealth) {
            health = newHealth;
        }

        // Use this for initialization
        void Start() {
            if (PhotonNetwork.isMasterClient) {
                health = initialHealth;
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
