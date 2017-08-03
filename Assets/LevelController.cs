using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class LevelController : MonoBehaviour {

        private RectTransform tr;

        // Use this for initialization
        void Start() {
            tr = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update() {

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
