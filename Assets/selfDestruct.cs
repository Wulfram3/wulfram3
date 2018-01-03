using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Wulfram3;

public class selfDestruct : Photon.PunBehaviour
{
   public GameObject selfDestructObject;
   public int bulletDamageinHitpoints = 1000;
    public bool fired = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        commitSD();
        if(!(selfDestructObject.GetComponentInChildren<ParticleSystem>().isPlaying)){
            selfDestructObject.SetActive(false);
            fired = false;
        }


        }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<HitPointsManager>() && (fired == true) && !(selfDestructObject.GetComponentInChildren<ParticleSystem>().isPlaying)) {
            HitPointsManager hitPointsManager = other.transform.GetComponent<HitPointsManager>();
            if (hitPointsManager != null)
            {
                hitPointsManager.TellServerTakeDamage(bulletDamageinHitpoints);


            }

        }
            

    }

        public void commitSD()
    {

        if (Input.GetKeyDown(KeyCode.V))
        {
            SyncSD();
            fired = true;
        }
    }


    public void SyncSD(){
        
            photonView.RPC("playParticles", PhotonTargets.All);
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
