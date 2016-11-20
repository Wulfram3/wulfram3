using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
    private RectTransform rect;
    private Vector2 healthRect = new Vector2(1, 1);

    // Use this for initialization
    void Start () {
        rect = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        rect.anchorMax = healthRect;
	}

    public void SetHealth(float health) {
        healthRect = new Vector2(health, 1);
    }
}
