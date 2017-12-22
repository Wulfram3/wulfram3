using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// Script to show how to controll a simple turret.
    /// </summary>
    public class SimpleTurretExampleController : MonoBehaviour {

	    public SimpleTurret simpleTurret;

	    public AbstractWeapon weapon;

	    private Camera mainCamera;

	    private Vector3 lastClickedPoint;

	    // Use this for initialization
	    void Start ()
        {
		    mainCamera = Camera.main;

		    simpleTurret.enabled = false; // At first, we don't want the turret enabled.

            weapon.ManualInitialization();
	    }
	
	    // Update is called once per frame
	    void Update ()
	    {
		    if(Input.GetMouseButtonDown(0))
		    {
			    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			    RaycastHit hit = new RaycastHit();
			    if(Physics.Raycast(ray,out hit))
			    {
				    if(!simpleTurret.enabled) simpleTurret.enabled = true;

				    lastClickedPoint = hit.point;
				    simpleTurret.TargetPosition = lastClickedPoint;
			    }
		    }

		    if(Input.GetKeyDown(KeyCode.Space))
		    {
			    weapon.Shoot(null,simpleTurret.cannonControllers[0].transform.forward);
		    }
	    }

	    void OnDrawGizmos()
	    {
		    Gizmos.color = Color.red;
		    if(lastClickedPoint != Vector3.zero)
		    {
			    Gizmos.DrawSphere(lastClickedPoint,0.3f);
		    }
	    }

	    void OnGUI()
	    {
		    GUILayout.Label("Click over sphere to aim to it");
		    GUILayout.Label("Press space to shoot.");
	    }
    }
}
