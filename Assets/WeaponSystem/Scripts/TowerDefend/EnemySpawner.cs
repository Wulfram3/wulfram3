using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class EnemySpawner : MonoBehaviour
	{
		public GameObject ObjectSpawn;
		public float ScaleMult = 1;
		public bool RandomScale = true;
		private float timeSpawnTemp = 0;
		public float TimeSpawn = 20;
		public float ObjectCount = 0;
		public int Radiun;
		public float Delay = 3;
		private int numberOfSpawned;
	
		private void Start ()
		{
			if (GetComponent<Renderer>())
				GetComponent<Renderer>().enabled = false;
		
			timeSpawnTemp = Time.time + Delay;	
		}

		private void Update ()
		{
			if (!ObjectSpawn)
				return;
		
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("Enemy");

			if (gos.Length < ObjectCount) {
				if (Time.time >= timeSpawnTemp + TimeSpawn) {
					GameObject enemyCreated = (GameObject)Instantiate (ObjectSpawn, transform.position + new Vector3 (Random.Range (-Radiun, Radiun), this.transform.position.y, Random.Range (-Radiun, Radiun)), Quaternion.identity);
              	
					if (enemyCreated.GetComponent<DamageManager> ())
						enemyCreated.GetComponent<DamageManager> ().HP += (int)(numberOfSpawned * 1.7f);
				
					if (enemyCreated.GetComponent<EnemyDead> ())
						enemyCreated.GetComponent<EnemyDead> ().MoneyPlus += (int)(numberOfSpawned * 0.1f);
				
					float scale = enemyCreated.transform.localScale.x;
					if (RandomScale)
						scale = Random.Range (0, 100) * 0.01f;
					enemyCreated.transform.localScale = new Vector3 (scale, scale, scale) * ScaleMult;

					timeSpawnTemp = Time.time;
					numberOfSpawned++;

				}
			}

		}

	}
}