using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class AutoCannon : Photon.PunBehaviour {
        public AudioClip autoCannonSound;
		public AudioClip shootCannonSound;
		public AudioSource audio;
        public int bulletDamageinHitpoints = 1;
        public float bulletsPerSecond = 10;
        public float range = 40;
        public float deviationConeRadius = 1;

        public bool debug = true;

        private float timeBetweenShots;
        private float lastFireTime;

        // Use this for initialization
        void Start() {
			
            timeBetweenShots = 1f / bulletsPerSecond;
        }


        // Update is called once per frame
        void Update() {
            if (!photonView.isMine)
                return;
            
			if (Input.GetMouseButton(1)) {

                float currentTime = Time.time;
                if (lastFireTime + timeBetweenShots > currentTime) {
                    return;
                }
				audio.PlayOneShot(autoCannonSound, 1);
                lastFireTime = currentTime;

                Vector3 pos = transform.position + (transform.forward * 1.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;

                Vector3 targetPoint = rotation * GetRandomPointInCircle();
                targetPoint += pos + transform.forward * range;
                if (debug) {
                    Debug.DrawLine(pos, targetPoint, Color.white, 1, false);
                }

                RaycastHit objectHit;
                Vector3 targetDirection = (targetPoint - pos).normalized;
                bool targetFound = Physics.Raycast(pos, targetDirection, out objectHit, range) && objectHit.transform.GetComponent<Unit>() != null;
                if (targetFound) {
                    HitPointsManager hitPointsManager = objectHit.transform.GetComponent<HitPointsManager>();
                    hitPointsManager.TellServerTakeDamage(bulletDamageinHitpoints);
                    print("autocannon hit");
					AudioSource.PlayClipAtPoint(autoCannonSound, transform.position);
                }

                

               
            }
        }

        private Vector3 GetRandomPointInCircle() {
            Vector2 randomPoint = Random.insideUnitCircle * deviationConeRadius;
            return new Vector3(randomPoint.x, randomPoint.y, 0);
        }
    }
}
