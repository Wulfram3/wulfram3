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
        public int fuelPerBullet = 1;
		private GameManager gameManager;
		//Start of the laser
		public Transform gunEnd;
        bool shooting = false;
		//camera for firing
		public Camera fpsCam;
        public float simDelayInSeconds = 0.1f;

        private float lastSimTime = 0;


		//wait for second on laser
		private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
		//line render for gun shots
		public LineRenderer laserLine;
		//next fire of laser
		private float nextFire;

        public bool debug = false;

        private float timeBetweenShots;
        private float lastFireTime;

        // Use this for initialization
        void Start() {



			//laser stuff
			laserLine = GetComponent<LineRenderer> ();
			fpsCam = GetComponentInParent<Camera> ();
			fpsCam = Camera.main;
			//original
            timeBetweenShots = 1f / bulletsPerSecond;


        }

		private GameManager GetGameManager() {
			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager>();
			}
			return gameManager;
		}


		private IEnumerator ShotEffect()
		{
			laserLine.enabled = true;
			yield return shotDuration;
			laserLine.enabled = false;

		}

        [PunRPC]
        public void SetShooting(bool newShootingValue) {
            if (!photonView.isMine) {
                shooting = newShootingValue;
            }   
        }

        // Update is called once per frame
        void Update() {

            if (photonView.isMine) {
                CheckAndFire();
            }
            ShowFeedback();
        }

        private void SetAndSyncShooting(bool newValue) {
            if (shooting != newValue) {
                shooting = newValue;
                SyncShooting();
            }
        }


        private void CheckAndFire() {
            if (Input.GetMouseButton(0)) {

                float currentTime = Time.time;
                if (lastFireTime + timeBetweenShots > currentTime ) {
                    return;
                }
                if (Cursor.visible || GetComponent<PlayerMovementManager>().isDead) {
                    SetAndSyncShooting(false);
                    return;
                }
                if (!GetComponent<FuelManager>().TakeFuel(fuelPerBullet)) {
                    SetAndSyncShooting(false);
                    return;
                }
                SetAndSyncShooting(true);

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
                //check if user is on same team
                //CHANGED HERE
                if (targetFound && objectHit.transform.GetComponent<Unit>().team != this.gameObject.GetComponent<Unit>().team) {
                    HitPointsManager hitPointsManager = objectHit.transform.GetComponent<HitPointsManager>();
                    if (hitPointsManager != null) {
                        hitPointsManager.TellServerTakeDamage(bulletDamageinHitpoints);
                        AudioSource.PlayClipAtPoint(autoCannonSound, transform.position);
                    }              
                }

            } else {
                SetAndSyncShooting(false);
            }
        }

        private void ShowFeedback() {
            if (shooting && (lastSimTime + simDelayInSeconds) < Time.time) {
                lastSimTime = Time.time;

                //Laser Effect
                StartCoroutine(ShotEffect());
                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;

                laserLine.SetPosition(0, gunEnd.position);

                Vector3 pos = transform.position + (transform.forward * 1.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;
                Vector3 bulletHitPoint;
                Vector3 targetPoint = rotation * GetRandomPointInCircle();
                targetPoint += pos + transform.forward * range;
                RaycastHit objectHit;
                Vector3 targetDirection = (targetPoint - pos).normalized;
                if (Physics.Raycast(rayOrigin, targetDirection, out hit, range)) {
                    bulletHitPoint = hit.point;
                } else {
                    bulletHitPoint = targetPoint;
                }
                laserLine.SetPosition(1, bulletHitPoint);

                //play sound
                audio.PlayOneShot(shootCannonSound, 1);
            }    
        }

        private void SyncShooting() {
            photonView.RPC("SetShooting", PhotonTargets.All, shooting);
        }

        private Vector3 GetRandomPointInCircle() {
            Vector2 randomPoint = Random.insideUnitCircle * deviationConeRadius;
            return new Vector3(randomPoint.x, randomPoint.y, 0);
        }


    }

}
