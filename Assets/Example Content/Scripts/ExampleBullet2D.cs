using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]           
    public class ExampleBullet2D : AbstractExampleBullet
    {
	    private Rigidbody2D myRigidbody;

	    protected override void Awake ()
	    {
		    base.Awake ();

		    myRigidbody = GetComponent<Rigidbody2D> ();
	    }

	    public override void Shoot(Vector3 _velocity, Transform _target, Vector3 _localHitOffset)
	    {
            target = _target;
            localHitOffset = _localHitOffset;
            speed = _velocity.magnitude;
            transform.up = _velocity.normalized;
            //myRigidbody.velocity = new Vector2(_velocity.x,_velocity.y);
        }

        protected override void Update()
        {
            Vector3 targetPosition = Vector3.forward;

            if(target == null)
                targetPosition = target.transform.position + localHitOffset;
            else
                targetPosition = transform.position + transform.up;

            Vector3 direction = targetPosition - transform.position;
            transform.up =  direction.normalized;
            transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);

            float sqrMagnitude = direction.sqrMagnitude;

            if (sqrMagnitude < 0.1f)
            {
                Enemy enemy = target.GetComponent<Enemy>();

                if (enemy != null)
                    enemy.Damage(bulletDamage);

                Explode(transform.forward * -1);
            }

            base.Update();
        }

        protected override void RaycastCollisionTest ()
	    {
		    float distance = myRigidbody.velocity.magnitude * Time.deltaTime * 1.1f;
		    Debug.DrawRay(transform.position,myRigidbody.velocity.normalized * distance,Color.magenta,0f);

		    RaycastHit2D hit = new RaycastHit2D ();
		    if(hit = Physics2D.Raycast(transform.position,myRigidbody.velocity.normalized,distance,layerMask))
		    {
			    Enemy enemy = hit.transform.GetComponent<Enemy>();
			
			    if(enemy != null)
				    enemy.Damage(bulletDamage);

			    Explode(transform.forward*-1f);
            }
	    }

    }
}
