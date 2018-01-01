using UnityEngine;

public class SmartPoolTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUILayout.Button("Spawn brick (click them to despawn)")) {
            var go = SmartPool.Spawn("Brick");
            if (go)
                go.transform.localPosition = Random.insideUnitSphere * 10;
        }
        if (GUILayout.Button("Despawn all bricks (and see them cull automatically)"))
            SmartPool.DespawnAllItems("Brick");


        if (GUILayout.Button("Spawn bullet (click them to despawn)")) {
            var go = SmartPool.Spawn("Bullet");
            if (go)
                go.transform.localPosition = Random.insideUnitSphere*10;
        }
        if (GUILayout.Button("Despawn all bullets"))
            SmartPool.DespawnAllItems("Bullet");
        GUILayout.Label("Please add Example and Example2ndScene as levels to the build settings!");
        if (GUILayout.Button("Switch Scene"))
            Application.LoadLevel(1);
        
        
    }
}
