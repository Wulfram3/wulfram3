using UnityEngine;
using System.Collections;


public class cargo : MonoBehaviour
{

    public AudioClip pickup;
    public AudioClip dropdown;
    public GameObject blueft;
    public bool isCreated;

	public GameObject pause;
    public bool redpcArea = false;


    void Update()
    {
        //This will toggle the enabled state of the two cameras between true and false each time
		if (Input.GetKeyDown("z"))
		{
			//drop item code
			transform.parent = Camera.main.transform;
			GetComponent<AudioSource>().PlayOneShot(dropdown, 1.0f);
			Renderer[] renderers = pause.GetComponentsInChildren<Renderer>();
			foreach (Renderer r in renderers) {
				r.enabled = true;
			}
			pause.transform.parent = null;
			//Debug.Log ("Turn cargo on");
		}
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "redpc")
        {
            Debug.Log("entered pc");
            redpcArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "redpc")
        {
            Debug.Log("exited red pc");
            redpcArea = false;
        }
    }




    public void OnTriggerStay(Collider other){

        if (Input.GetKeyDown("q"))
        {

			if (other.tag.Contains("Red"))
            { // only be picked by the player!
				isCreated = false;
                
                GetComponent<AudioSource>().PlayOneShot(pickup, 1.0f);
				pause = GameObject.Find(other.gameObject.transform.name);
				Debug.Log (pause);
				pause.transform.parent = Camera.main.transform;
				Debug.Log (other.gameObject.transform.name);
				Renderer[] renderers = pause.GetComponentsInChildren<Renderer>();
				foreach (Renderer r in renderers) {
					r.enabled = false;
				}
					//Debug.Log ("Turn cargo off");
            }


        }
			


		if (Input.GetKeyDown (",")) {
           
			if (other.tag == "RedCargo" && !isCreated && redpcArea == true) {
				PhotonNetwork.Instantiate ("FlakTurret", new Vector3 (transform.position.x, transform.position.y + 1.4f, transform.position.z), Quaternion.identity, 0);
				isCreated = true;
                Debug.Log("Flak Turret Created");
                GameObject.Destroy(pause);
            } else if (other.tag == "RedRepairPad" && !isCreated && redpcArea == true)
            {

                PhotonNetwork.Instantiate ("RepairPad", new Vector3 (transform.position.x, transform.position.y + 1.4f, transform.position.z), Quaternion.identity, 0);
				isCreated = true;
                GameObject.Destroy(pause);

            }

			

				
		}
        }
}
