using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// Turret idle state. Turret is waiting to adquire a new target
    /// </summary>
    public class TurretIdleState : FSMState 
    {	
	    SuperTurret turretActor=null;
	
	    public TurretIdleState(GameObject obj):base(obj)
	    {
		    stateID = StateID.Idle;
		    turretActor = obj.GetComponent<SuperTurret>();
	    }
	

	    public override void DoBeforeEntering ()
	    {
		    base.DoBeforeEntering ();
	    }
	
	    public override void Reason (GameObject player)
	    {
		    //Test if turret has a target go to deploying state
		    if(turretActor.Target != null || turretActor.HasPointToShootIgnoringAllConditions())
		    {
			    turretActor.GetMachineState().PerformTransition(Transition.TargetInRange);
		    }
	    }
	
	    public override void Act (GameObject player)
	    {
		    // Try to choose a new target each frame we are in idle state
		    turretActor.ChooseNewTarget();
	    }
	
	    public override void DoBeforeLeaving ()
	    {		
		    base.DoBeforeLeaving();
	    }

    }
}
