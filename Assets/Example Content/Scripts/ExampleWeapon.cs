using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
    /// <summary>
    /// Example weapon provided with SuperTurrets.
    /// </summary>
    public class ExampleWeapon : AbstractWeapon
    {			
	    public AudioClip 		shotSound;
	
	    public float			shotFrequency;
	
	    public AbstractExampleBullet bullet;
	
	    public float			shotSpeed;
	
	    public ParticleSystem	shootParticlesPrefab;
	
	    public Light			shootLightPrefab;


	    private float			timeCounter = 0f;

	    private ParticleSystem  particlesInstance;

	    private Light 			lightInstance;

	    private AudioSource 	shotAudioSource;

	    public override void ManualInitialization()
	    {
		    particlesInstance = Instantiate(shootParticlesPrefab) as ParticleSystem;
		    particlesInstance.transform.SetParent (shootPoint);
		    particlesInstance.transform.localPosition = Vector3.zero;
		    particlesInstance.transform.localRotation = Quaternion.identity;
		    particlesInstance.Stop (); // Just in case it's as Play on Awake

		    if(shootLightPrefab != null)
		    {
			    lightInstance = Instantiate(shootLightPrefab) as Light;
			    lightInstance.transform.SetParent (shootPoint);
			    lightInstance.transform.localPosition = Vector3.zero;
			    lightInstance.transform.localRotation = Quaternion.identity;
			    lightInstance.enabled = false;
		    }

            shotAudioSource = shootPoint.gameObject.AddComponent<AudioSource>();
        }

	    // Update is called once per frame
	    void Update ()
	    {
		    if (timeCounter > shotFrequency && !WeaponReady)
		    {
			    WeaponReady = true;
		    }
		    else
		    {
			    timeCounter += Time.deltaTime;	
		    }
	    }

	    #region implemented abstract members of AbstractWeapon

	    public override void Shoot (GameObject target, Vector3 _direction, Vector3 _localOffset)
	    {
		    //if(WeaponReady)
		    {
			    WeaponReady = false;

                Transform targetTransform = target == null ? null : target.transform;
			    // Note: In a real project, you must not instrante many objects a runtime, instead, you may consider creating a bullets pool.
			    AbstractExampleBullet newBullet = Instantiate(bullet,shootPoint.position,Quaternion.identity) as AbstractExampleBullet;
			    newBullet.Shoot(_direction.normalized * shotSpeed, targetTransform, _localOffset);
			
			    if(shotSound != null && shotAudioSource != null)
				    shotAudioSource.PlayOneShot(shotSound);
			
			    if(particlesInstance != null)
				    particlesInstance.Play();
			
			    StartCoroutine(LightFlash());
			
			    // Reset timmer
			    timeCounter = 0f;
		    }
	    }
	
	    public override void Shoot (Vector3 _position, Vector3 _direction, Vector3 _localOffset)
	    {
		    Shoot (null,_direction, _localOffset);
	    }
	
	    private IEnumerator LightFlash()
	    {
		    if (shootLightPrefab != null)
		    {
			    lightInstance.enabled = true;
			    yield return new WaitForSeconds(0.1f);
			    lightInstance.enabled = false;
			
		    }
		
		    yield return new WaitForSeconds(0f);
	    }
	
	    #endregion
    }
}
