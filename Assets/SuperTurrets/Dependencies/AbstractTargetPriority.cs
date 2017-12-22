using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OptimizedGuy
{
/// <summary>
/// Each class that implements this one will have a different criteria for getting a new target.
/// </summary>
public abstract class AbstractTargetPriority : ScriptableObject
{
	/// <summary>
	/// The method to check if target is in range (distance, angle, etc)
	/// </summary>
	public delegate bool IsTargeReadyDelegate(GameObject newTarget);
	
	/// <summary>
	/// Orders all targets by priority. Each algorithm will apply a different criteria for ordering
	/// </summary>
	public abstract List<GameObject> OrderTargetsByPriority(List<GameObject> availibleTargets);
	
	public virtual GameObject GetTarget(List<GameObject> availibleTargets, IsTargeReadyDelegate IsTargetReady,ref GameObject bestCandidateTarget)
	{
		List<GameObject> orderedTargets = OrderTargetsByPriority(availibleTargets);
		
		if(orderedTargets.Count > 0)
			// Best candidate target is always the first of ordered targets by priority 
			bestCandidateTarget = orderedTargets[0];
		
		// Check if target is in range and return it, the first returned target will have the highest priority 
		for(int i = 0; i< orderedTargets.Count; i++)
		{
			GameObject target = orderedTargets[i];
		
			if(IsTargetReady(target))
				return target;
		}
		
		// No target is ready..
		return null;
	}
}
}
