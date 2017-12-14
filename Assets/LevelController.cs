using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class LevelController : MonoBehaviour {

        private RectTransform tr;
        private bool updatedOnFirstFrame = false;

        // Use this for initialization
        void Start() {
            tr = GetComponent<RectTransform>();
            
        }

        // Update is called once per frame
        void Update() {
            //TODO: hack/fix, doesn't work in Start, gets rect.width as 0
            if (!updatedOnFirstFrame) {
                SetLevel(1);
                updatedOnFirstFrame = true;
            }
        }

        public void SetLevel(float level) {
            tr.anchorMax = new Vector2(0, 1);
            tr.pivot = new Vector2(0, 0.5f);
            float maxWidth = transform.parent.GetComponent<RectTransform>().rect.width;
            float newWidth = maxWidth * level;
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            Debug.Log("NEwWidth " + newWidth + " level " + level);
        }
    }
}
