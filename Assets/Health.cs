using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int maxHitpoints = 100;
    public int hitpoints = 0;

	// Use this for initialization
	void Start () {
        Reset();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Damage(int points)
    {
        if (hitpoints - points < 0)
        {
            hitpoints = 0;
        } else
        {
            hitpoints -= points;
        }
        
    }

    void Heal(int points)
    {
        if (hitpoints + points > maxHitpoints)
        {
            hitpoints = maxHitpoints;
        } else
        {
            hitpoints += points;
        }      
    }

    void Reset()
    {
        hitpoints = maxHitpoints;
    }
}
