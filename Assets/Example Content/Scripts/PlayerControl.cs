using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class PlayerControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	 public float speed = 50.0F;
    public float rotationSpeed = 100.0F;
	// Update is called once per frame
	void Update () {
		
		float vertical = Input.GetAxis("Vertical") * speed;
        float horizontal = Input.GetAxis("Horizontal") * speed;
        vertical *= Time.deltaTime;
		horizontal *= Time.deltaTime;
        transform.Translate(horizontal, vertical, 0);
	}
}
}
