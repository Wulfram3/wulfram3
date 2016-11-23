using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Effects;

public class PulseShell : NetworkBehaviour {
    public GameObject explosionPrefab;
    public float velocity = 30;
    public int directHitpointsDamage = 40; 

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
        Combat combat = col.gameObject.GetComponent<Combat>();
        if (combat != null)
        {
            combat.TakeDamage(directHitpointsDamage);
        }

        Vector3 pos = col.contacts[0].point;
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = pos;
        Destroy(explosion, 3);
        NetworkServer.Spawn(explosion);
        Destroy(gameObject);
    }
}
