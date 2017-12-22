using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class RTSUnit : MonoBehaviour {

	UnityEngine.AI.NavMeshAgent agent;

	private Quaternion targetRotation;

	void Awake()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	// Use this for initialization
	void Start ()
	{
		GetNetDestination();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!agent.hasPath)
		{
			GetNetDestination();
		}
	}
	
	void GetNetDestination()
	{
		UnityEngine.AI.NavMeshHit hit = new UnityEngine.AI.NavMeshHit ();
		
		Vector3 newPosition = Vector3.zero + new Vector3 (Random.Range (-25, 25), 0f, Random.Range (-25, 25));
		if(UnityEngine.AI.NavMesh.SamplePosition (newPosition, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
		{
			agent.SetDestination(hit.position);
		}
	}
}
}
