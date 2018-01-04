using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class TargetController : Photon.PunBehaviour {

        private GameManager gameManager;
        public GameObject[] targets;

        private int currentTarget;
        private int totalTargets;

        public Transform target;
        public Texture2D image;

        Vector3 point;

        // Use this for initialization
        void Start() {
            gameManager = FindObjectOfType<GameManager>();
            targets = GameObject.FindGameObjectsWithTag("Unit");
        }

        // Update is called once per frame
        void Update() {
            if (!photonView.isMine)
                return;

            var units = (Unit[])GameObject.FindObjectsOfType(typeof(Unit));

            if (Input.GetKeyDown(KeyCode.T)) {
                Vector3 pos = transform.position + (transform.forward * 2.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;

                RaycastHit objectHit;
                bool targetFound = Physics.Raycast(pos, transform.forward, out objectHit, 300) && objectHit.transform.GetComponent<Unit>() != null;
                if (targetFound) {
                    gameManager.SetCurrentTarget(objectHit.transform.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentTarget = currentTarget + 1 % targets.Length;
                //target = targets[currentTarget];
                gameManager.SetCurrentTarget(targets[currentTarget]);           
            }
        }
    }
}
