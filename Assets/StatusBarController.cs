using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarController : MonoBehaviour {

    public Text statusText1;

    public Text statusText2;

    float pingCount;
    float frameCount;
    // Use this for initialization
    IEnumerator Start () {
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);

                pingCount = PhotonNetwork.GetPing();
                frameCount = Mathf.Round((1 / Time.deltaTime));
                statusText1.text = "Ping:" + pingCount + " ~ " + "FPS:" + frameCount;
            }
            else
            {
                statusText1.text = "Ping:pause ~ FPS:pause";
            }


            yield return new WaitForSeconds(0.5f);
        }
    }
	
	// Update is called once per frame
	void Update () {

    }
}
