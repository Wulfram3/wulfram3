using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.Wulfram3 {
    public class CameraManager : Photon.PunBehaviour {
        public Vector3 thirdPersonPos = new Vector3(0, 1, -2.5f);
        public Vector3 firstPersonPos = new Vector3(0, 0.33f, -0.3f);
        public float transitionTime = 1.0f;

        private Camera cam;
        private Vector3 currentPos;
        private Vector3 targetPos;
        private float transitionStartTime;
        private bool transitionComplete = true;

        // Use this for initialization
        void Start() {
            if (photonView.isMine) {
                targetPos = thirdPersonPos;
                currentPos = targetPos;
                cam = Camera.main;
                cam.transform.SetParent(transform);
                cam.transform.localPosition = currentPos;
                cam.transform.rotation = Quaternion.identity;
            }
        }

        // Update is called once per frame
        void Update() {
            if (photonView.isMine) {
                if (Input.GetKeyDown(KeyCode.C)) {
                    SwapTargetpos();
                    transitionStartTime = Time.time;
                    transitionComplete = false;
                }
                if (!transitionComplete) {
                    float fracComplete = (Time.time - transitionStartTime) / transitionTime;
                    if (fracComplete >= 1.0f) {
                        transitionComplete = true;
                        currentPos = targetPos;
                    } else {
                        currentPos = Vector3.Slerp(currentPos, targetPos, fracComplete);
                    }
                    cam.transform.localPosition = currentPos;
                }

            }
        }

        private void SwapTargetpos() {
            if (targetPos.Equals(thirdPersonPos)) {
                targetPos = firstPersonPos;
            } else {
                targetPos = thirdPersonPos;
            }
        }
    }
}
