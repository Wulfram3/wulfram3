using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class PulseShellManager : Photon.PunBehaviour {
        public GameObject explosionPrefab;
        public float velocity = 30;
        public int directHitpointsDamage = 40;
        public float maxLifeTime = 5;

        private float lifeTime = 0;

        // Use this for initialization
        void Start() {
            if (PhotonNetwork.isMasterClient) {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = transform.forward * velocity;            
            }
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

        void OnCollisionEnter(Collision col) {
            if (PhotonNetwork.isMasterClient) {
                //Combat combat = col.gameObject.GetComponent<Combat>();
                //if (combat != null) {
                //    combat.TakeDamage(directHitpointsDamage);
                //}

                //Vector3 pos = col.contacts[0].point;
                //GameObject explosion = Instantiate(explosionPrefab);
                //explosion.transform.position = pos;
                //Destroy(explosion, 3);
                //NetworkServer.Spawn(explosion);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
