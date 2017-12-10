using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Wulfram3 {
    public class TargetInfoController : MonoBehaviour {

        public GameObject targetInfoPanel;
        public Text hitpoints;
        public Text name;
        public Text team;

        private GameObject target;
        private Vector3 pos;

        // Use this for initialization
        void Start() {
            FindObjectOfType<GameManager>().AddTargetChangeListener(this);
            TargetChanged(null);
        }

        // Update is called once per frame
        void LateUpdate() {
            if (target != null && target.GetComponentInChildren<Renderer>().isVisible) {
                targetInfoPanel.SetActive(true);
                pos = Camera.main.WorldToScreenPoint(target.transform.position);
                pos.z = 0;
                RectTransform rectTransform = GetComponent<RectTransform>();
                pos.y -= 20;
                //rectTransform.localPosition = new Vector2(0, 100);
                rectTransform.SetPositionAndRotation(pos, rectTransform.rotation);

                hitpoints.text = target.GetComponent<HitPointsManager>().health + "/" + target.GetComponent<HitPointsManager>().maxHealth;
                name.text = target.GetComponent<Unit>().name;
                team.text = "" + target.GetComponent<Unit>().team;
            } else {
                targetInfoPanel.SetActive(false);
            }
        }

        public void TargetChanged(GameObject t) {
            target = t;
            if (t == null) {
                targetInfoPanel.SetActive(false);
            } else {
                targetInfoPanel.SetActive(true);
            }
        }

        public void OnDrawGizmos() {
            if (target != null) {
                //drawString("blah", target.transform.position, Color.blue);
            }
        }

        //static public void drawString(string text, Vector3 worldPos, Color? colour = null) {
        //    UnityEditor.Handles.BeginGUI();

        //    var restoreColor = GUI.color;

        //    if (colour.HasValue) GUI.color = colour.Value;
        //    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        //    if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0) {
        //        GUI.color = restoreColor;
        //        UnityEditor.Handles.EndGUI();
        //        return;
        //    }

        //    Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        //    GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, size.x, size.y), text);
        //    GUI.color = restoreColor;
        //    UnityEditor.Handles.EndGUI();
        //}

        //static public void drawString2(string text, Vector3 worldPos, Color? colour = null) {
        //    UnityEditor.Handles.BeginGUI();

        //    var restoreColor = GUI.color;

        //    if (colour.HasValue) GUI.color = colour.Value;
        //    var view = UnityEditor.SceneView.currentDrawingSceneView;
        //    Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        //    if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0) {
        //        GUI.color = restoreColor;
        //        UnityEditor.Handles.EndGUI();
        //        return;
        //    }

        //    Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        //    GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        //    GUI.color = restoreColor;
        //    UnityEditor.Handles.EndGUI();
        //}
    }
}
