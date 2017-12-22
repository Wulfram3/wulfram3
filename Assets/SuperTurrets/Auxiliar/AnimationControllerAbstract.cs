using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    public abstract class AnimationControllerAbstract : MonoBehaviour
    {
        public float deploySpeed = 1f;

        public float foldSpeed = 1f;

        /// <summary>
        /// Play animations of the gameObject forward
        /// </summary>
        public abstract void PlayAnimationForward();

        /// <summary>
        /// Play the animations of the game object backward
        /// </summary>
        public abstract void PlayAnimationBackward();

        /// <summary>
        /// Is idle?
        /// </summary>
        public abstract bool IsIdle();

        /// <summary>
        /// Is deployed?
        /// </summary>
        public abstract bool IsDeployed();

        /// <summary>
        /// Caled after a transition ends to reset whatever you need.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Toggle On/off animation/animator.
        /// </summary>
        public abstract void Toggle(bool _toggle);

        /// <summary>
        /// SKipping transition we set the turret in deployed state
        /// </summary>
        public abstract void SetDeployedState();
    }
}