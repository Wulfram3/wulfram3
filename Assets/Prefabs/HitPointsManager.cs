using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class HitPointsManager : Photon.PunBehaviour {

        public int initialHealth = 100;
        public int maxHealth = 100;

        public int health;
        private GameManager gameManager;

        public void TakeDamage(int amount) {
            if (PhotonNetwork.isMasterClient) {
                int newHealth = Mathf.Clamp(health - amount, 0, maxHealth);
                photonView.RPC("UpdateHealth", PhotonTargets.All, newHealth);
                Debug.Log("Hitpoints: " + newHealth);
            }
        }

        [PunRPC]
        public void UpdateHealth(int newHealth) {
            health = newHealth;
            if (tag.Equals("Player") && photonView.isMine) {
                gameManager.SetHullBar((float)health / (float)maxHealth);
            }
        }

        // Use this for initialization
        void Start() {
            gameManager = FindObjectOfType<GameManager>();
            if (PhotonNetwork.isMasterClient) {
                health = initialHealth;
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
