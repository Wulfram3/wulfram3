using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class FlakTurretManager : Photon.PunBehaviour {

        public float reloadTime = 5;
        public float turnSpeed = 10;
        public float scanInterval = 3;
        public float scanRadius = 300;
        public float testTargetOnSightInterval = 0.5f;

        private GameManager gameManager;
        private float timeSinceLastScan = 0;
        private Transform currentTarget = null;
        private bool targetOnSight = false;
        private float timeSinceLastFire = 0;

        // Use this for initialization
        void Start() {
            if (PhotonNetwork.isMasterClient) {
                gameManager = FindObjectOfType<GameManager>();
            }
        }

        // Update is called once per frame
        void Update() {
            if (PhotonNetwork.isMasterClient) {
                FindTarget();
                TurnTowardsCurrentTarget();
                CheckTargetOnSight();
                FireAtTarget();
            }
        }

        private void FireAtTarget() {
            timeSinceLastFire += Time.deltaTime;
            if (timeSinceLastFire >= reloadTime && targetOnSight) {
                Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;
                gameManager.SpawnPulseShell(pos, rotation);
                timeSinceLastFire = 0;
            }
        }

        private void CheckTargetOnSight() {
            if (currentTarget == null) {
                targetOnSight = false;
                return;
            }

            RaycastHit objectHit;
            Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
            targetOnSight = Physics.Raycast(pos, transform.forward, out objectHit, scanRadius) && objectHit.collider.transform.Equals(currentTarget);
        }

        private void TurnTowardsCurrentTarget() {
            if (currentTarget != null) {
                Vector3 lookPos = currentTarget.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            }
        }

        private void FindTarget() {
            timeSinceLastScan += Time.deltaTime;
            if (timeSinceLastScan >= scanInterval) {
                Transform closestTarget = null;
                float minDistance = scanRadius + 10f;

                var cols = Physics.OverlapSphere(transform.position, scanRadius);
                var rigidbodies = new List<Rigidbody>();
                foreach (var col in cols) {
                    if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody) && col.tag.Equals("Player")) {
                        rigidbodies.Add(col.attachedRigidbody);
                    }
                }

                foreach (Rigidbody rb in rigidbodies) {
                    Transform target = rb.transform;

                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance < minDistance) {
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
