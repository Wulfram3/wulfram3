using UnityEngine;
using System.Collections;

public class charactercontroller_sidescroller : MonoBehaviour
{
	private float itsVelocity = 200.0f;
	private Rigidbody itsRigidBody = null;
	private bool itsGrounded = true;
	
	void Awake()
	{
		itsRigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			itsRigidBody.AddForce(-itsVelocity,0.0f,0.0f,ForceMode.Force);
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			itsRigidBody.AddForce(itsVelocity,0.0f,0.0f,ForceMode.Force);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space) && itsGrounded)
		{
			GetComponent<Rigidbody>().AddForce(0.0f,1500.0f,0.0f);
			itsGrounded = false;
		}
		if(itsRigidBody.velocity.x > 5.0f)
		{
			itsRigidBody.velocity = new Vector3(5.0f,itsRigidBody.velocity.y,0.0f);
		}
		else if(itsRigidBody.velocity.x < -5.0f)
		{
			itsRigidBody.velocity = new Vector3(-5.0f,itsRigidBody.velocity.y,0.0f);
		}
		itsRigidBody.AddForce(0.0f,-50.0f,0.0f);	//gravity
		
		if(transform.position.y < -10.0f)
			itsRigidBody.MovePosition(new Vector3(transform.position.x,20.0f,0.0f));
	}
	
	public void OnCollisionEnter(Collision theCollision)
	{
		itsGrounded = true;
	}
}
