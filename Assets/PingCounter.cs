using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingCounter : MonoBehaviour {

    string label = "";
    float count;

    IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);

                label = "PING:" + PhotonNetwork.GetPing().ToString();
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
        GUI.Label(new Rect(5, 70, 100, 25), label);
    }
}