using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class ExampleBullet3D : AbstractExampleBullet
    {
	    public GameObject bulletLight;

        private float radius;


	    protected override void Awake()
	    {
		    base.Awake ();
		    bulletLight.SetActive(false);
            radius = GetComponent<SphereCollider>().radius;
        }

	    public override void Shoot(Vector3 _velocity, Transform _target, Vector3 _localHitOffset)
	    {
            target = _target;
		    transform.forward = _velocity.normalized;
            localHitOffset = _localHitOffset;
            speed = _velocity.magnitude;
        }

        protected override void Update()
        {
            Vector3 targetPosition = Vector3.zero;

            if (target == null) 
            {
                // Go forward if target is null
                targetPosition = transform.position + transform.forward;
            }
            else
            {
                targetPosition = target.transform.position + localHitOffset;
            }

            transform.LookAt(targetPosition);
            Vector3 direction = targetPosition - transform.position;
            transform.Translate(direction.normalized * Time.deltaTime * speed,Space.World);

            float sqrMagnitude = direction.sqrMagnitude;

            if (sqrMagnitude < 0.2f)
            {
                if (target != null)
                {
                    Enemy enemy = target.GetComponent<Enemy>();

                    if (enemy != null)
                        enemy.Damage(bulletDamage);
                }

                Explode(transform.forward * -1);
            }

            base.Update();
        }

        protected override void Explode(Vector3 hitDirection)
	    {
		    base.Explode (hitDirection);
		
		    if(bulletExplosion != null)
		    {
			    bulletLight.transform.parent = null;
			    bulletLight.SetActive(true);
			    Destroy(bulletLight,0.1f);
		    }
            else
		    {
			    Destroy(gameObject);
		    }
	    }

	    protected override void RaycastCollisionTest ()
	    {
            float distance = radius*2f;
            Vector3 direction = transform.forward;
		    Debug.DrawRay(transform.position, direction * distance,Color.magenta,0f);

		    RaycastHit hit = new RaycastHit ();

		    if(Physics.Raycast(transform.position, direction, out hit,distance,layerMask))
		    {
			    Enemy enemy = hit.transform.GetComponent<Enemy>();

			    if(enemy != null)
				    enemy.Damage(bulletDamage);

			    Explode(direction * -1f);
		    }
	    }

        void OnDrawGizmos()
        {
            if (target == null) return;

            Vector3 targetPosition = target.transform.position + localHitOffset;
            Gizmos.DrawSphere(targetPosition, 0.35f);
        }
    }
}
