using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// Turret is ready to shot and shot if it can.
    /// </summary>
    public class TurretAttackingState : FSMState
    {

        private SuperTurret turretActor = null;

        public TurretAttackingState(GameObject npc) : base(npc)
        {
            stateID = StateID.Attacking;
            turretActor = npc.GetComponent<SuperTurret>();
        }

        public override void DoBeforeEntering()
        {
            turretActor.bodyController.enabled = true;

            if (turretActor.animationController != null && turretActor.disableAnimatorWhenAttacking)
                turretActor.animationController.Toggle(false);

            foreach (var cannon in turretActor.cannons)
            {
                if (cannon != null)
                    cannon.cannonController.enabled = true;
            }

            base.DoBeforeEntering();
        }

        public override void DoBeforeLeaving()
        {
            if (turretActor.animationController != null && turretActor.disableAnimatorWhenAttacking)
            {
                turretActor.animationController.Toggle(true);
                turretActor.animationController.SetDeployedState(); // Needed because animator is resetted when disabled
            }


            base.DoBeforeLeaving();
        }

        public override void Reason(GameObject target)
        {
            if (target == null && !turretActor.HasPointToShootIgnoringAllConditions())
            {
                turretActor.GetMachineState().PerformTransition(Transition.LostTarget);
            }
        }

        public override void Act(GameObject newTarget)
        {
            if (turretActor.HasPointToShootIgnoringAllConditions())
            {
                ShootAtPointIgnoringAllConstraints(turretActor.GetPointToShootIgnoringAllConditions());
            }
            else if (newTarget != null)
            {
                Shoot(newTarget, false);
            }
        }

        public void ForceShot(GameObject newTarget)
        {
            Shoot(newTarget, true);
        }

        /// <summary>
        /// Shots all trurrets cannons. newTarget can be null, only is useful for bullets like missiles, that needs a target.
        /// </summary>
        /// <param name='forceShot'>
        /// If true, it will ignore visibility tests and turret will shot.
        /// </param>
        private void Shoot(GameObject newTarget, bool forceShot)
        {
            if (forceShot || (turretActor.IsTargetReady(newTarget) && turretActor.ReadyToShoot))
            {
                if (!forceShot && !turretActor.bodyController.IsPointingTarget())
                    return;

                foreach (var cannon in turretActor.cannons)
                {
                    if (cannon == null)
                        return;

                    if (!forceShot && cannon.weapon != null && !cannon.weapon.WeaponReady)
                        continue;

                    Vector3 localHitOffset = Vector3.zero;

                    Vector3 dir = turretActor.GetMode() == Mode.game3D ? cannon.cannonController.transform.forward : cannon.cannonController.transform.up;

                    // Do a detailed visibility test for each cannon if needed.
                    // Only cannons with a target in sight will shot 
                    if (!forceShot && turretActor.visibilityLevel == VisibilityPrecissionLevel.Detailed)
                    {
                        Vector3 origin = cannon.cannonController.transform.position;

                        if (turretActor.debugMode)
                            Debug.DrawRay(origin, dir.normalized * turretActor.attackArea.Radius, Color.white, Time.deltaTime);

                        if (!turretActor.VisibilityTest(origin, dir, turretActor.attackArea.Radius, newTarget, ref localHitOffset))
                        {
                            continue;
                        }
                    }

                    // In case we avoid raycasting, we have to check cannon angle to avoid cannons shotting to nothing
                    if (!forceShot && turretActor.visibilityLevel == VisibilityPrecissionLevel.None)
                    {
                        if (!cannon.cannonController.IsPointingTarget())
                            continue;
                    }

                    // Calculate bullets offset based on cannon offsets from center
                    if (cannon.shootPoint != null && ( turretActor.visibilityLevel == VisibilityPrecissionLevel.None || turretActor.visibilityLevel == VisibilityPrecissionLevel.Simple))
                    {
                        Vector3 centerOfShootPoints = GetCenterOfShootPoints();

                        localHitOffset = cannon.shootPoint.position - centerOfShootPoints;
                    }

                    // Fire at will !!!!!!!!
                    if (cannon.weapon != null)
                    {
                        cannon.weapon.Shoot(newTarget, dir, localHitOffset);

                        if (cannon.recoil && cannon.recoilAnchor != null)
                            cannon.RecoilController.DoRecoilAnimation();
                    }
                }
            }
        }

        /// <summary>
        /// Using this method we force the turret to shot to the given point ignoring al constraints (visibility	/// </summary>
        /// <returns>The at point ignoring all constraints.</returns>
        /// <param name="_point">_point.</param>
        private void ShootAtPointIgnoringAllConstraints(Vector3 _point)
        {
            if (!turretActor.bodyController.IsPointingTarget())
                return;

            foreach (var cannon in turretActor.cannons)
            {
                if (cannon == null)
                    return;

                if (cannon.weapon != null && !cannon.weapon.WeaponReady)
                    continue;

                // Fire at will !!!!!!!!
                if (cannon.weapon != null)
                {
                    Vector3 direction = turretActor.GetMode() == Mode.game3D ? cannon.cannonController.transform.forward : cannon.cannonController.transform.up;
                    cannon.weapon.Shoot(_point, direction);
                }

                // Recoil
                if (cannon.recoil && cannon.recoilAnchor != null)
                    cannon.RecoilController.DoRecoilAnimation();
            }
        }

        /// <summary>
        /// Gets the center of all shot points.
        /// </summary>
        private Vector3 GetCenterOfShootPoints()
        {
            float x = 0, y = 0, z = 0;
            foreach (var cannon in turretActor.cannons)
            {
                if (cannon.shootPoint == null)
                {
                    return GetCenterOfShootCannons();
                }
                x += cannon.shootPoint.position.x;
                y += cannon.shootPoint.position.y;
                z += cannon.shootPoint.position.z;
            }

            float total = turretActor.cannons.Length;
            return new Vector3(x / total, y / total, z / total);
        }

        /// <summary>
        /// Gets the center of all cannons. Used if shoot points aren't assigned because this turret isn't using weapons.
        /// </summary>
        private Vector3 GetCenterOfShootCannons()
        {
            float x = 0, y = 0, z = 0;
            foreach (var cannon in turretActor.cannons)
            {
                x += cannon.cannonController.transform.position.x;
                y += cannon.cannonController.transform.position.y;
                z += cannon.cannonController.transform.position.z;
            }

            float total = turretActor.cannons.Length;
            return new Vector3(x / total, y / total, z / total);
        }
    }
}
