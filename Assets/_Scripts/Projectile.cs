using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private BoundCheck bndCheck;

	// Use this for initialization
	void awake() {
        bndCheck = GetComponent<BoundCheck>();
	}
	
	// Update is called once per frame
	void Update () {
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }
	}
}
