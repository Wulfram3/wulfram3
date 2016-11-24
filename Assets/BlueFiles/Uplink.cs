using UnityEngine;
using System.Collections;

public class Uplink : MonoBehaviour {
	
	
	
	
	public AudioClip pickup;
	public AudioClip dropdown;
	public GameObject blueft;
	void Update () {
		
		if (Input.GetKeyDown (",")) {
						//drop item code
						GetComponent<AudioSource>().PlayOneShot (dropdown, 1.0f);
						transform.GetComponent<Renderer>().enabled = true;
						transform.parent = null;
				} 
	
		
	}
	
	
	
	

	
	
	void  OnTriggerEnter(Collider player) {
		
		/*if (!hasBox){
				
				hasBox = true;
				
				box.transform.parent = player.transform;
				
			}else{
				
				Destroy(box.gameObject);
				
			}*/
		
	}
	
	
	
	
	void OnTriggerStay(Collider other){
		bool canClone = true;
		
		if (Input.GetKeyDown ("z")) {
			//drop item code
			GetComponent<AudioSource>().PlayOneShot (dropdown, 1.0f);
			transform.GetComponent<Renderer>().enabled = true;
			transform.parent = null;
		} 
		
		

		
		
		
		if(Input.GetKeyDown("q")){
			
			if (other.tag == "Player"){ // only be picked by the player!
				
				transform.parent = Camera.main.transform; 
				transform.GetComponent<Renderer>().enabled = false;// pick this object...
				GetComponent<AudioSource>().PlayOneShot(pickup, 1.0f);
				
			}
			
			
		}
		
	}
}	