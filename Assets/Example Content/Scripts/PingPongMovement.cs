using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class PingPongMovement : MonoBehaviour 
{	
	public  Vector3 	axis;
	public  float 		time;
	public  float		displacement;
	
	private float 		currentTime;
	
	// Update is called once per frame
	void Update ()
	{
		if(currentTime >= time)
		{
			axis 		*= -1;
			currentTime = 0f;
		}
	
		currentTime	+= Time.deltaTime;
		
		transform.Translate(axis*displacement*Time.deltaTime);
	}
}
}
