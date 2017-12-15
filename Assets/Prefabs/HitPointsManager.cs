﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class HitPointsManager : Photon.PunBehaviour {

        public int initialHealth = 100;
        public int maxHealth = 100;

        [HideInInspector]
        public int health;
        private GameManager gameManager;

        public void TakeDamage(int amount) {
            if (PhotonNetwork.isMasterClient) {
                int newHealth = Mathf.Clamp(health - amount, 0, maxHealth);
                SetHealth(newHealth);
                Debug.Log("Hitpoints: " + newHealth);
            }
        }

        [PunRPC]
        public void UpdateHealth(int newHealth) {
            health = newHealth;
            gameManager.UnitsHealthUpdated(this);
        }

        public void SetHealth(int newHealth) {
            if (PhotonNetwork.isMasterClient) {
                photonView.RPC("UpdateHealth", PhotonTargets.AllBuffered, newHealth);
            }
        }

        [PunRPC]
        public void SetHealthFromClient(int newHealth)
        {
            // Can do validation here later
            SetHealth(newHealth);
        }

        public void TellServerHealth(int newHealth)
        {
            photonView.RPC("SetHealthFromClient", PhotonTargets.MasterClient, newHealth);
        }

        // Use this for initialization
        void Start() {
            gameManager = FindObjectOfType<GameManager>();
            if (PhotonNetwork.isMasterClient) {
                SetHealth(initialHealth);
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
