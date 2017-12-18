using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3{
public class RayViewer : Photon.PunBehaviour {
	public float weaponRange = 50f;
	private Camera fpsCam;



	void Start () {
		fpsCam = GetComponentInParent<Camera> ();
		fpsCam = Camera.main;
	}







	void Update () {



		
		Vector3 lineOrigin = fpsCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 0));
		Debug.DrawRay (lineOrigin, fpsCam.transform.forward * weaponRange, Color.green);


		
	}
	
	

}
}