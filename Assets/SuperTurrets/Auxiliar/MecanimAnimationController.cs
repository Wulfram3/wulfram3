using UnityEngine;
using System.Collections;
using System;

namespace OptimizedGuy
{
    /// <summary>
    /// Helper class to control turrets animations
    /// </summary>
    [AddComponentMenu("OptimizedGuy/SuperTurrets/Mecanim Animation Controller")]
    public class MecanimAnimationController : AnimationControllerAbstract {
		
	    public string baseLayer = "Base Layer",idleStateName,deployedStateName,deployTriggerName,foldTriggerName,deploySpeedParameterName,foldSpeedParameterName;

	    public Animator animator;

        public int selectedLayerIndex = 0, deployParameterIndex = 0, foldParameterIndex = 0, idleStateIndex, deployedStateIndex, deployTriggerIndex,foldTriggerIndex;

	    private AnimationState animationState;

	    private int idleHash,deployedHash;

	    void Awake()
	    {
		    // Cache names to avoid memory allocation due to string compositions
		    idleHash 			= Animator.StringToHash(baseLayer+"."+idleStateName);
		    deployedHash 		= Animator.StringToHash(baseLayer+"."+deployedStateName);

            animator.SetFloat(deploySpeedParameterName, deploySpeed);
            animator.SetFloat(foldSpeedParameterName, foldSpeed);
        }


	    /// <summary>
	    /// Play animations of the gameObject forward
	    /// </summary>
	    public override void PlayAnimationForward()
	    {
		    animator.SetTrigger(deployTriggerName);
	    }

        /// <summary>
        /// Play the animations of the game object backward
        /// </summary>
        public override void PlayAnimationBackward()
        {
            animator.SetTrigger(foldTriggerName);
        }


        public override bool IsIdle()
        {
            bool idle = animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleHash;

            return idle;
        }

        public override bool IsDeployed()
        {
            bool deployed = animator.GetCurrentAnimatorStateInfo(0).fullPathHash == deployedHash;

            return deployed;
        }

        public override void Reset()
        {
            GetComponent<Animator>().ResetTrigger(deployTriggerName);
            GetComponent<Animator>().ResetTrigger(foldTriggerName);
        }

        public override void Toggle(bool _toggle)
        {
            animator.enabled = _toggle;
        }

        public override void SetDeployedState()
        {
            animator.Play(deployedHash);
        }
    }
}
