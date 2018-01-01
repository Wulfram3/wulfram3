using UnityEngine;
using System.Collections;

public class DespawnOnClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        SmartPool.Despawn(gameObject);
    }
}
