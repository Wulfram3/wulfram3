using UnityEngine;
using System.Collections;


public class cargo : MonoBehaviour
{

    public AudioClip pickup;
    public AudioClip dropdown;
    public GameObject blueft;
    public bool isCreated;

	public GameObject pause;



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
			Debug.Log ("Turn cargo on");
		}
    }





    public void OnTriggerStay(Collider other){
        bool canClone = true;

 


        if (Input.GetKeyDown("q"))
        {

            if (other.tag == "BlueCargo")
            { // only be picked by the player!

                
                GetComponent<AudioSource>().PlayOneShot(pickup, 1.0f);
				pause = GameObject.Find(other.gameObject.transform.name);
				pause.transform.parent = Camera.main.transform;
				Debug.Log (other.gameObject.transform.name);
				Renderer[] renderers = pause.GetComponentsInChildren<Renderer>();
				foreach (Renderer r in renderers) {
					r.enabled = false;
				}
					Debug.Log ("Turn cargo off");
            }


        }
			


        if (Input.GetKeyDown(","))
        {
           
                if (pause.tag == "BlueCargo" && !isCreated)
                {
                    PhotonNetwork.Instantiate("FlakTurret", new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z), Quaternion.identity, 0);
                    isCreated = true;

                }
                    GameObject.Destroy(pause);

                
            }
        }
}
