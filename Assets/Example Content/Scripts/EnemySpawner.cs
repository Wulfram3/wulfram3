using UnityEngine;
using System.Collections;

namespace OptimizedGuy
{
public class EnemySpawner : MonoBehaviour 
{
	public  GameObject  enemyToSpawn;
	public  float 		coolDown;
	public KeyCode		keyToSpawnEnemy;
	private float 		actualTime=0; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{	
		actualTime+= Time.deltaTime;
		
		if(Input.GetKeyDown(keyToSpawnEnemy))
		{
			if(actualTime>=coolDown)
			{
				actualTime=0;
				Instantiate(enemyToSpawn,transform.position,Quaternion.identity);
			}
		}
	}
}
}