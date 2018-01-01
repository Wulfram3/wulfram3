using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoBehaviour {

    public Font myFont;
    GUIStyle style;
    string label = "";
    float count;

    IEnumerator Start()
    {
        style = new GUIStyle();
        style.font = myFont;
        style.fontSize = 12;
        style.normal.textColor = Color.yellow;
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                count = (1 / Time.deltaTime);
                label = "FPS:" + (Mathf.Round(count));
            }
            else
            {
                label = "Pause";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 40, 100, 25), label, style);
    }
}


