using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class TargetController : Photon.PunBehaviour {

        private GameManager gameManager;

        // Use this for initialization
        void Start() {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update() {
            if (!photonView.isMine)
                return;

            if (Input.GetKeyDown(KeyCode.T)) {
                Vector3 pos = transform.position + (transform.forward * 2.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;

                RaycastHit objectHit;
                bool targetFound = Physics.Raycast(pos, transform.forward, out objectHit, 300) && objectHit.transform.GetComponent<Unit>() != null;
                if (targetFound) {
                    gameManager.SetCurrentTarget(objectHit.transform.gameObject);
                }
            }
        }
    }
}
