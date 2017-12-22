using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OptimizedGuy
{
/// <summary>
/// Target selection strategy where we select the most dangerous enemy.
/// </summary>
public class StrongerTargetingPriority : AbstractTargetPriority {

	public override List<GameObject> OrderTargetsByPriority (List<GameObject> availibleTargets)
	{
		List<GameObject> res = new List<GameObject>(availibleTargets.Count);
		
		res.AddRange(availibleTargets);
		
		for (int i = 0; i < res.Count; i++)
		{
			for (int j = i; j < res.Count; j++)
			{
				AbstractEnemy enemy1 = res[i].GetComponent<AbstractEnemy>();
				AbstractEnemy enemy2 = res[j].GetComponent<AbstractEnemy>();
				
				if(enemy1 != null && enemy2 != null)
				{
					if(enemy2.GetHazard() > enemy1.GetHazard())
					{
						// Change order
						GameObject aux 	= res[j];
						res[j] 			= res[i];
						res[i]			= aux;
					}
				}else
				{
					Debug.LogError("Enemies must have a script inherting from AbstractEnemy to use this algorithm");
				}
			}
		}
		
		return res;
	}
}
}
