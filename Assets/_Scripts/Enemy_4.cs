using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Part
{
    //these three fields need to be defined in the Inspector pane
    public string name; //the name of this part
    public float health; //the amount of health this part has
    public string[] protectedBy; //the other parts that protect this

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat; //the Material to show damage
}

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector:Enemy_4")]
    public Part[] parts;

    public Vector3 p0, p1;
    public float timeStart;
    public float duration = 4;

    // Use this for initialization
    void Start() {
        p0 = p1 = pos;

        InitMovement();
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }

    }
    void InitMovement()
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;

        p1.x = Random.Range(-widMinRad,widMinRad);
        p1.y = Random.Range(-hgtMinRad,hgtMinRad);

     

        //reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        //this completely overrides Enemy.Move() with a linear interpolation

        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            //if u >=1
            InitMovement(); //then initializes movement to a new point
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // apply ease out easing to u

        pos = (1 - u) * p0 + u * p1; //simple linear interpolation
    }

    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            //if no real Part was passed in
            return (true);
        }
        //returns the result of the comparison, prt.health <= 0
        //if prt.health is 0 or less, returns true (yes, it will be destroyed)
        return (prt.health <= 0);
    }
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;

        showingDamage = true;
    }


    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //enemies don't take damage unless they're on screen
                //this stops the player from shooting them before they are visible
                
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                //hurt this enemy
               
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    //if prtHit wasn't found then it's usually because, very rarely, thisCollider on
                    //contacts[0] will be the ProjectileHero instead of the ship part
                    //if so, just look for otherCollider instead
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //check whether this part is still protected
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        //if one of the protecting parts hasn't been destroyed
                        if (!Destroyed(s))
                        {
                            //then don't dmage this part yet
                            Destroy(other); //destory this ProjectileHero
                            return; //return before causing damage
                        }
                    }
                }
                
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                //show dmaange on the part
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    //instead of destroying this enemy, disable the enemy part
                    prtHit.go.SetActive(false);
                }
                //check to see if the whole ship is destroyed
                bool allDestroyed = true; //assume it is destroyed
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        //if a part still exists
                        allDestroyed = false; //change allDestroyed to false
                        break; //and break out of the foreach loop
                    }
                }
                if (allDestroyed)
                {
                    //if it IS completely destroyed
                    Main.S.ShipDestroyed(this);
                    //destroy this enemy
                    Destroy(this.gameObject);
                }
                Destroy(other); //destroy the ProjectileHero
                break;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
