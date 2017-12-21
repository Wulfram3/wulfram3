using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3
{
    public class HealthRegenerator : Photon.PunBehaviour
    {

        public float healthPerSecond = 0.1f;

        private float healthCollected = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.isMine)
            {
                float health = healthPerSecond * Time.deltaTime;
                healthCollected += health;
                if (healthCollected >= 1f)
                {
                    healthCollected--;
                    GetComponent<HitPointsManager>().TakeDamage(-1);
                }
            }
        }
    }
}