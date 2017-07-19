using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class GUIManager : MonoBehaviour {
        private bool wasVisible;

        // Use this for initialization
        void Start() {
            wasVisible = Cursor.visible;
            SetChildrenActive(wasVisible);
        }

        private void SetChildrenActive(bool active) {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(active);
            }
        }

        // Update is called once per frame
        void Update() {
            if (Cursor.visible != wasVisible) {
                wasVisible = Cursor.visible;
                SetChildrenActive(wasVisible);
            }
        }
    }
}
