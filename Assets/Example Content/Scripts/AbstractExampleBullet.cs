using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    public abstract class AbstractExampleBullet : MonoBehaviour
    {
	    public ParticleSystem bulletExplosion;
	    public float bulletDamage = 1f;
	    public LayerMask layerMask;

        protected Transform target;

        protected Vector3 localHitOffset;

        protected float speed;


	    /// <summary>
	    /// Square distance to delete bullet if is to this distance from camera.
	    /// </summary>
	    private const float SQR_DISTANCE_TO_DELETE = 10000f;

	
	    // Cache vars for performance
	    private Transform myTransform;
	    private Transform cameraTransform;

	    protected virtual void Awake()
	    {
		    myTransform 	= transform;
		    cameraTransform = Camera.main.transform;
		    name = "Bullet "+Time.time.ToString("00.00");
	    }
	
	    public abstract void Shoot (Vector3 _velocity, Transform _target, Vector3 _localHitOffset = default(Vector3));

	    // Update is called once per frame
	    protected virtual void Update ()
	    {
		    RaycastCollisionTest();

		    if(Vector3.SqrMagnitude(myTransform.position-cameraTransform.position) > SQR_DISTANCE_TO_DELETE)
		    {
			    Destroy(gameObject);
		    }
	    }

	    void OnCollisionEnter(Collision _col)
	    {
		    Explode (_col.contacts[0].normal);
	    }

	    void OnCollisionEnter2D(Collision2D _col)
	    {
		    Explode (_col.contacts[0].normal);
	    }

	    protected virtual void Explode(Vector3 hitDirection)
	    {
		    if(bulletExplosion != null)
		    {
			    bulletExplosion.gameObject.SetActive(true);
			    bulletExplosion.transform.parent = null;
			    bulletExplosion.transform.LookAt(Vector3.Reflect(hitDirection,hitDirection));
			    bulletExplosion.Play();

			    Destroy(bulletExplosion.gameObject,0.4f);
			    Destroy(gameObject);
			
		    }else
		    {
			    Destroy(gameObject);
		    }
	    }

	    protected abstract void RaycastCollisionTest();
    }
}
