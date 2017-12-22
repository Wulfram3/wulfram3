using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class TurretDeadState : FSMState 
{
	public TurretDeadState(GameObject npc) :base(npc)
	{
		
	}
	
	#region implemented abstract members of FSMState
	public override void Reason(GameObject player)
	{
		//No reason needed 
	}
	
	public override void Act(GameObject player)
	{
		
	}
	
	#endregion
}
}