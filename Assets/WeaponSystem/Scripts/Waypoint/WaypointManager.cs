using UnityEngine;
using System.Collections;

public class WaypointManager : MonoBehaviour {

	public Waypoint[] Waypoints;
	void Start () {
		Waypoints = (Waypoint[])GameObject.FindObjectsOfType(typeof(Waypoint));
	}
	
	void Update () {
	
	}
}
