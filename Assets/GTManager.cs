using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3
{
    public class GTManager : Photon.PunBehaviour
    {

        public float reloadTime = 10;
        public float turnSpeed = 10;
        public float scanInterval = 3;
        public float scanRadius = 10;
        public float testTargetOnSightInterval = 0.5f;
        public int bulletDamageinHitpoints = 5;
        public string teamDetect;

        private GameManager gameManager;
        private float timeSinceLastScan = 0;
        private Transform currentTarget = null;
        private bool targetOnSight = false;
        private float timeSinceLastFire = 0;

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////
        /// </summary>

        //line render gt stuff
        //Start of the laser
        public Transform gunEnd;
        bool shooting = false;
        public float simDelayInSeconds = 0.1f;

        private float lastSimTime = 0;

        public float deviationConeRadius = 1;
        public float range = 40;
        //wait for second on laser
        private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
        //line render for gun shots
        public LineRenderer laserLine;
        //next fire of laser
        private float nextFire;

        public bool debug = false;

        private float timeBetweenShots;
        private float lastFireTime;
        public float bulletsPerSecond = 10;
        /// <summary>
        /// ///////////////////////
        /// </summary>

        // Use this for initialization
        void Start()
        {


            //gt stuff
            //laser stuff
            laserLine = GetComponent<LineRenderer>();
            //original
            timeBetweenShots = 1f / bulletsPerSecond;

            if (PhotonNetwork.isMasterClient)
            {
                gameManager = GetGameManager();
            }
        }

        private GameManager GetGameManager()
        {
            if (gameManager == null)
            {
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
        public void SetShooting(bool newShootingValue)
        {
            if (!photonView.isMine)
            {
                shooting = newShootingValue;
            }
        }
        private void SyncShooting()
        {
            photonView.RPC("SetShooting", PhotonTargets.All, shooting);
        }

        private void ShowFeedback()
        {
            if (shooting && Time.time - lastSimTime >= timeBetweenShots)
            {

                lastSimTime = Time.time;
                StartCoroutine(ShotEffect());
                laserLine.SetPosition(0, gunEnd.position);
                laserLine.SetPosition(1, currentTarget.transform.position);
                //play sound
                // audio.PlayOneShot(shootCannonSound, 1);
            }
        }

        private Vector3 GetRandomPointInCircle()
        {
            Vector2 randomPoint = Random.insideUnitCircle * deviationConeRadius;
            return new Vector3(randomPoint.x, randomPoint.y, 0);
        }
        private void SetAndSyncShooting(bool newValue)
        {
            if (shooting != newValue)
            {
                shooting = newValue;
                SyncShooting();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.isMasterClient)
            {
                FindTarget();
                TurnTowardsCurrentTarget();
                CheckTargetOnSight();
                FireAtTarget();
            }
            ShowFeedback();
        }

        private void FireAtTarget()
        {
            timeSinceLastFire += Time.deltaTime;
            if (timeSinceLastFire >= reloadTime && targetOnSight)
            {
                Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;
                //GetGameManager().SpawnFlakShell(pos, rotation);
                if (currentTarget == null)
                {
                    targetOnSight = false;
                    return;
                }

                RaycastHit objectHit;
                Vector3 post = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
                targetOnSight = Physics.Raycast(post, transform.forward, out objectHit, scanRadius) && objectHit.collider.transform.Equals(currentTarget);
                if (targetOnSight && objectHit.transform.GetComponent<Unit>().team != this.gameObject.GetComponent<Unit>().team)

                {
                    HitPointsManager hitPointsManager = objectHit.transform.GetComponent<HitPointsManager>();
                    if (hitPointsManager != null)
                    {
                        hitPointsManager.TellServerTakeDamage(bulletDamageinHitpoints);

                    }
                }
                timeSinceLastFire = 0;
            }
        }
        /* RaycastHit objectHit;
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
                    */
        private void CheckTargetOnSight()
        {
            if (currentTarget == null)
            {
                targetOnSight = false;
                return;
            }

            RaycastHit objectHit;
            Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
            targetOnSight = Physics.Raycast(pos, transform.forward, out objectHit, scanRadius) && objectHit.collider.transform.Equals(currentTarget);
            var distance = Vector3.Distance(objectHit.transform.position, this.gameObject.transform.position);
            //Debug.Log("Distance between:" + distance);
            if (distance >= 24.14107)
            {
                targetOnSight = false;
                return;
            }
            if (targetOnSight)
            {
                SetAndSyncShooting(true);
            }
            else
            {
                SetAndSyncShooting(false);
            }
        }


        private void TurnTowardsCurrentTarget()
        {
            if (currentTarget != null)
            {
                Vector3 lookPos = currentTarget.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            }
        }

        private void FindTarget()
        {

            timeSinceLastScan += Time.deltaTime;
            if (timeSinceLastScan >= scanInterval)
            {
                Transform closestTarget = null;
                float minDistance = scanRadius + 1f;

                var cols = Physics.OverlapSphere(transform.position, scanRadius);
                var rigidbodies = new List<Rigidbody>();
                foreach (var col in cols)
                {
                    if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody) && col.tag.Equals("Player") && col.GetComponent<Unit>().team == teamDetect)
                    {
                        rigidbodies.Add(col.attachedRigidbody);
                    }
                }

                foreach (Rigidbody rb in rigidbodies)
                {
                    Transform target = rb.transform;

                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = target;
                    }
                }

                currentTarget = closestTarget;

                timeSinceLastScan = 0;
            }
        }
    }
}

