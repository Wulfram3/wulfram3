using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class EnemySpawnScene : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
	{
		//Time.timeScale = 10f;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKey(KeyCode.J))
			Time.timeScale = 0.3f;
		
		if(Input.GetKey(KeyCode.K))
			Time.timeScale = 1f;
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,10,300,50),"Pres 1, 2 or 3 to spawn enemies");	
	}
}
}