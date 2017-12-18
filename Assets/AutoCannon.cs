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
		private GameManager gameManager;
		//Start of the laser
		public Transform gunEnd;
		//camera for firing
		public Camera fpsCam;


		//wait for second on laser
		private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
		//line render for gun shots
		public LineRenderer laserLine;
		//next fire of laser
		private float nextFire;

        public bool debug = true;

        private float timeBetweenShots;
        private float lastFireTime;

        // Use this for initialization
        void Start() {

			gameManager = FindObjectOfType<GameManager>();

			//laser stuff
			laserLine = GetComponent<LineRenderer> ();
			fpsCam = GetComponentInParent<Camera> ();
			fpsCam = Camera.main;
			//original
            timeBetweenShots = 1f / bulletsPerSecond;


        }



		private IEnumerator ShotEffect()
		{
			laserLine.enabled = true;
			yield return shotDuration;
			laserLine.enabled = false;

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
				//Laser Effect
				StartCoroutine (ShotEffect ());
				Vector3 rayOrigin = fpsCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 0));
				RaycastHit hit;

				laserLine.SetPosition (0, gunEnd.position);
				if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, range)) {
					laserLine.SetPosition (1, hit.point);
				} else {
					laserLine.SetPosition (1, rayOrigin + (fpsCam.transform.forward * range));

				}





				//play sound
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
