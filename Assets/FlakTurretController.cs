using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class FlakTurretController : NetworkBehaviour
{
    public float reloadTime = 5;
    public GameObject pulseShellPrefab;

    private float lastFireTime = 0;
    public Vector3 origin;

	// Use this for initialization
	void Start () {
        origin = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	    if (!isServer)
        {
            return;
        }

       

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
 
        foreach (GameObject target in objs)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < 40)
            {
                Vector3 lookPos = target.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);

                if (Time.fixedTime > reloadTime + lastFireTime)
                {
                    lastFireTime = Time.fixedTime;
                    CmdFirePulseShell();
                }
            }
        }
	}

    void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }
        UpdatePos();
    }

    private void UpdatePos()
    {
        if (Vector3.Distance(transform.position, origin) > 0.1f)
        {
            transform.GetComponent<Rigidbody>().AddForce((origin - transform.position) * 1000f * Vector3.Distance(origin, transform.position));
            //Debug.Log(Vector3.Distance(origin, transform.position));
        }
        
    }

    [Command]
    void CmdFirePulseShell()
    {
        GameObject pulseShell = GameObject.Instantiate(pulseShellPrefab);
        pulseShell.transform.position = transform.position;
        pulseShell.transform.rotation = transform.rotation;
        pulseShell.transform.Translate(Vector3.forward);
        NetworkServer.Spawn(pulseShell);
    }
}
