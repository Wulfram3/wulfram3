using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    public class MouseControlledTarget : MonoBehaviour {
	
	    public Transform rotationPivot;

        public Transform rotateTarget;
	
	    public SimpleTurret	targetTurret;

        public AbstractWeapon weapon;

        public Transform shootPoint;
	
	    public float	 mouseSensivity = 1f;
	
	    private Vector3 oldMousePosition;

        void Awake()
        {
            weapon.ManualInitialization();
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 currentPosition = Input.mousePosition;

            Vector3 direction = oldMousePosition - currentPosition;
            direction.Normalize();

            if (direction.x != 0)
                rotateTarget.RotateAround(rotationPivot.position, Vector3.down, mouseSensivity * direction.x * Time.deltaTime);

            if (direction.y != 0)
                rotateTarget.RotateAround(rotationPivot.position, Vector3.forward, mouseSensivity * direction.y * Time.deltaTime);

            oldMousePosition = currentPosition;
	    }

        void LateUpdate()
        {
            targetTurret.SetShootingPointIgnoringAllConstraints(rotateTarget.position, true);

            if (Input.GetMouseButtonDown(0))
            {
                weapon.Shoot(null,shootPoint.forward);
            }
        }
    }
}
