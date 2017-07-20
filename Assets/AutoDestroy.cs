using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class AutoDestroy : Photon.PunBehaviour {
        public float maxLifeTime = 5;
        private float lifeTime = 0;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (PhotonNetwork.isMasterClient) {
                lifeTime += Time.deltaTime;
                if (lifeTime >= maxLifeTime) {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }
}
