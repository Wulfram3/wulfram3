using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// State for turret deploying animation
    /// </summary>
    public class TurretDeployingState : FSMState {
	
	    private AnimationControllerAbstract animationController;
	    private SuperTurret turretActor;
	    bool deploying=false;
	
	    public TurretDeployingState(GameObject npc,AnimationControllerAbstract animationController):base(npc)
	    {
		    stateID = StateID.Deploying;
		    turretActor = npc.GetComponent<SuperTurret>();
		    this.animationController = animationController;
	    }
	
	    public override void Reason (GameObject player)
	    {
		    // Is deploying and animation has finished ? Or turret has not animation ?
 		    if(animationController == null || (deploying && animationController.IsDeployed()))
		    {
                if(animationController != null)
                    animationController.Reset();

			    turretActor.GetMachineState().PerformTransition(Transition.DeployingEnd);
		    }	
	    }
	
	    public override void Act (GameObject player)
	    {
		    // Start deploying animation if not started yet
		    if(animationController != null && animationController.IsIdle())
		    {
			    animationController.PlayAnimationForward();
			
			    deploying=true;
		    }
	    }
	
	    public override void DoBeforeLeaving ()
	    {
		    //If deploying animation ends...
		    deploying=false;
		    base.DoBeforeLeaving();
	    }
    }
}
