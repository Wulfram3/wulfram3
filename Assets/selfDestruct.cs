using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Wulfram3;

public class selfDestruct : Photon.PunBehaviour
{
    /*  CHEEB NOTES
     *  This script can likely be adapted into a general splash damage module
     */
    // selfDestructObject should be set in editor. Empty game object with child effects
    public GameObject selfDestructObject;
    public int maximumDamage = 1000; // Will only be applied at a range of 0 from epicenter
    public float sphereRadius = 10f;   // Distance in all directions damage will be applied

    private bool fired = false;        // Internal flag to track state

    void Start() { }

    void Update()
    {
        if (photonView.isMine) // Prevent firing on all equipped vehicles
        {
            if (fired && !selfDestructObject.GetComponentInChildren<ParticleSystem>().isPlaying && selfDestructObject.GetActive()) // Verify user has fired, effects have played, and a final check to make sure the object is indeed active
            {
                doDamage();
            }
            if (!fired && Input.GetKeyDown(KeyCode.V)) // Detect user input, disallow repeat triggering with 'fired' flag
            {
                fired = true;
                photonView.RPC("playParticles", PhotonTargets.All); // Activates the effects on this tank for all players
            }
        }
    }

    private void doDamage()
    {
        HitPointsManager myHPM = transform.GetComponent<HitPointsManager>();
        if (myHPM) // Just some error checking to make sure we can die
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, sphereRadius); // Collect a list of all hit objects
            int i = 0;
            while (i < targets.Length) // Cycle through hit objects
            {
                HitPointsManager hpm = targets[i].transform.GetComponent<HitPointsManager>();
                if (hpm) // Prevent trying to damage objects that do not have hitpoints
                {
                    // (range - distance) / range gives a float from 0 to 1 that increases as distance to center of explosion decreases
                    float multi = (sphereRadius - Vector3.Distance(transform.position, targets[i].transform.position)) / sphereRadius;
                    hpm.TellServerTakeDamage((int)Mathf.Ceil(maximumDamage * multi));
                }
                i++;
            }
            myHPM.TellServerTakeDamage(maximumDamage); // This kills us
            selfDestructObject.SetActive(false); // These final two lines make sure we don't fire again at respawn
            fired = false;
        }
    }

    [PunRPC]
    public void playParticles()
    {
        if (selfDestructObject.transform && !(selfDestructObject.GetComponentInChildren<ParticleSystem>().isPlaying))
        {
            selfDestructObject.SetActive(true);
        }
    }
}
