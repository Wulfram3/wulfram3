using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace UnityStandardAssets.Effects {
    public class Combat : NetworkBehaviour
    {
        private HealthBar healthBar;

        public const int maxHealth = 100;
        [SyncVar(hook = "OnChangeHealth")]
        public int health = maxHealth;

        public void TakeDamage(int amount) {
            if (!isServer)
            {
                return;
            }

            health -= amount;
            if (health <= 0) {
                health = 0;
                Debug.Log("Dead! Respawning");
                health = maxHealth;
                RpcRespawn();
            }           
            
        }

        void OnChangeHealth(int currentHealth)
        {
            if (isLocalPlayer)
            {
                float h = (float)currentHealth / (float)maxHealth;
                healthBar.SetHealth(h);
                Debug.Log("Health: " + string.Format("{0:#,###.##}", h * 100.0) + "%");
            }
            
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                // move back to zero location
                transform.position = Vector3.zero;
            }
        }

        // Use this for initialization
        void Start() {
            healthBar = GameObject.FindObjectOfType<HealthBar>();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
