using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	void Start () {
		if(GetComponent<Renderer>())
		GetComponent<Renderer>().enabled = false;
	}

}
