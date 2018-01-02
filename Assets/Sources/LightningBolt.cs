/*
	This script is placed in public domain. The author takes no responsibility for any possible harm.
	Contributed by Jonathan Czeck
*/
using UnityEngine;
using System.Collections;
namespace Com.Wulfram3 {
public class LightningBolt : Photon.PunBehaviour
{
	public GameObject target;
	public int zigs = 100;
	public float speed = 1f;
	public float scale = 1f;
	public Light startLight;
	public Light endLight;


	Perlin noise;
	float oneOverZigs;
	
	private Particle[] particles;
	
	void Start()
	{
			target = new GameObject();
		GetComponent<ParticleEmitter>();
		oneOverZigs = 1f / (float)zigs;
		GetComponent<ParticleEmitter>().emit = false;

		GetComponent<ParticleEmitter>().Emit(zigs);
		particles = GetComponent<ParticleEmitter>().particles;
	}
	
	void Update ()
	{


            //if (!photonView.isMine)
            //return;

            if (Input.GetMouseButtonDown(1)){
			Vector3 pos = transform.position + (transform.forward * 2.0f + transform.up * 0.2f);
			Quaternion rotation = transform.rotation;

			RaycastHit objectHit;
			bool targetFound = Physics.Raycast(pos, transform.forward, out objectHit, 300) && objectHit.transform.GetComponent<Unit>() != null;
			if (targetFound) {
					//set gameobject transform for repairbeam
				target = objectHit.transform.gameObject;
			}
		}


		if (noise == null)
			noise = new Perlin();
			
		float timex = Time.time * speed * 0.1365143f;
		float timey = Time.time * speed * 1.21688f;
		float timez = Time.time * speed * 2.5564f;
		
		for (int i=0; i < particles.Length; i++)
		{
			Vector3 position = Vector3.Lerp(transform.position, target.transform.position, oneOverZigs * (float)i);
			Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
										noise.Noise(timey + position.x, timey + position.y, timey + position.z),
										noise.Noise(timez + position.x, timez + position.y, timez + position.z));
			position += (offset * scale * ((float)i * oneOverZigs));
			
			particles[i].position = position;
			particles[i].color = Color.white;
			particles[i].energy = 1f;
		}
		
		GetComponent<ParticleEmitter>().particles = particles;
		
		if (GetComponent<ParticleEmitter>().particleCount >= 2)
		{
			if (startLight)
				startLight.transform.position = particles[0].position;
			if (endLight)
				endLight.transform.position = particles[particles.Length - 1].position;
		}
	}	
}
}