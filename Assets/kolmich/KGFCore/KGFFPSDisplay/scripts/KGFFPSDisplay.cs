// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2013-02-21</date>
// <summary>Simple FPS display</summary>

using UnityEngine;
using System.Collections;

public class KGFFPSDisplay : MonoBehaviour
{
	float itsFPS = 0;
	int itsFrameCounter = 0;
	float itsLastMeasurePoint = 0;
	public float itsTimeBetweenMeasurePoints = 2;
	public int itsFontSize = 30;
	public Color itsFontColor = Color.white;
	
	GUIStyle itsStyleText;
	
	void Start()
	{
		itsStyleText = new GUIStyle();
		itsStyleText.fontSize = itsFontSize;
		itsStyleText.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		itsFrameCounter ++;
		if (Time.time - itsLastMeasurePoint > itsTimeBetweenMeasurePoints)
		{
			itsFPS = itsFrameCounter / (Time.time - itsLastMeasurePoint);
			
			itsFrameCounter = 0;
			itsLastMeasurePoint = Time.time;
		}
	}
	
	void OnGUI()
	{
		GUI.color = Color.black;
		GUI.Label(new Rect(1,1,200,200),""+((int)itsFPS)+" FPS",itsStyleText);
		GUI.color = itsFontColor;
		GUI.Label(new Rect(0,0,200,200),""+((int)itsFPS)+" FPS",itsStyleText);
	}
}
