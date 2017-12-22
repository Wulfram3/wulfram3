using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
/// <summary>
/// Area is a helper class that inform to suscribers about targets entering or exiting from this area.
/// </summary>
public class Area : MonoBehaviour 
{	
	// Target Enter
	public delegate void TargetEnterHandler(GameObject target, Area area);
	public event TargetEnterHandler TargetEnterEvent;
	
	// Target Exit
	public delegate void TargetExitHandler(GameObject target, Area area);
	public event TargetExitHandler TargetExitEvent;
	
	// Area radius
	public float Radius {get; private set;}
	
	/// <summary>
	/// Only objects with tag contained in importantTags will be notified to turret.
	/// </summary>
	private string[] importantTags;
	
	
	void Awake()
	{
		if(GetComponent<SphereCollider>() != null)
			Radius = GetComponent<SphereCollider>().radius;
		else if(GetComponent<CircleCollider2D>() != null)
			Radius = GetComponent<CircleCollider2D>().radius;
	}
	
	/// <summary>
	/// Radar reports to turret about targets entering in radar area
	/// </summary>
	/// <param name="targetCollider">
	/// Object inside radar
	/// </param>
	public void OnTriggerEnter(Collider targetCollider)
	{
		if (TargetEnterEvent != null && IsTagImportant(targetCollider.tag))
		{
			//Debug.Log(name+"--"+targetCollider.name+"--ENter");
			TargetEnterEvent(targetCollider.gameObject,this);
		}
	}
	
	/// <summary>
	/// Radar reports to turret about targets exiting radar area
	/// </summary>
	/// <param name="targetCollider">
	/// Object outisde radar
	/// </param>
	public void OnTriggerExit(Collider targetCollider)
	{
		if (TargetExitEvent != null && IsTagImportant(targetCollider.tag))
		{
			//Debug.Log(name+"--"+targetCollider.name+"--Exit");
			TargetExitEvent(targetCollider.gameObject,this);
		}
	}

	void OnTriggerEnter2D(Collider2D targetCollider)
	{
		if (TargetEnterEvent != null && IsTagImportant(targetCollider.tag))
		{
			//Debug.Log(name+"--"+targetCollider.name+"--ENter");
			TargetEnterEvent(targetCollider.gameObject,this);
		}
	}

	void OnTriggerExit2D(Collider2D targetCollider)
	{
		if (TargetExitEvent != null && IsTagImportant(targetCollider.tag))
		{
			//Debug.Log(name+"--"+targetCollider.name+"--Exit");
			TargetExitEvent(targetCollider.gameObject,this);
		}
	}
	
	/// <summary>
	/// Radar reports to turret about targets inside radar area.
	/// Needed because some times, a target die, and then we need get a new target that already is inside the area.
	/// </summary>
	/// <param name="a">
	/// Object inside radar.
	/// </param>
	/*void OnTriggerStay(Collider targetCollider)
	{
		if (TargetEnterEvent != null && IsTagImportant(targetCollider.tag))
		{
			TargetEnterEvent(targetCollider.gameObject,this);
		}
	}*/
	
	/// <summary>
	/// Used by turret actor to notify area its enemy tags.
	/// </summary>
	public void SetImportantTags(string[] turretEnemyTags)
	{
		importantTags = turretEnemyTags;	
	}
	
	/// <summary>
	/// Determines if newTag is contained in importantTags
	/// </summary>
	private bool IsTagImportant(string newTag)
	{
		bool res = false;

		if(importantTags == null || (importantTags != null && importantTags.Length == 0))
			return true;
		
		foreach (string tag in importantTags)
		{
			if(tag == newTag)
			{
				res = true;
				break;
			}
		}
		
		return res;
	}
}
}
