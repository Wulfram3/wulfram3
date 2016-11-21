using UnityEngine;
using UnityEngine.Networking;

public class PulseShell : NetworkBehaviour {
    public GameObject explosionPrefab;
    public float velocity = 30;

	// Use this for initialization
	void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * velocity;
        Destroy(gameObject, 5f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col) {
        Vector3 pos = col.contacts[0].point;
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = pos;
        Destroy(explosion, 3);
        NetworkServer.Spawn(explosion);
        Destroy(gameObject);
    }
}
