using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Set in Inspector Enemy")]
    public float speed = 10f; //the speed in m/s
    public float fireRate = 0.3f; //seconds/shot (Unused)
    public float health = 10;
    public int score = 100; //points earned for destroying this

    private BoundCheck bndCheck;



    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundCheck>();
    }




    // Update is called once per frame
    void Update () {
        Move();
        if (bndCheck != null && bndCheck.offDown){

           
                Destroy(gameObject);
            
        }
    }


    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGo = coll.gameObject;
        if (otherGo.tag == "projectileHero")
        {
            Destroy(otherGo);
            Destroy(gameObject);

        }
        else
        {
            print("Enemy hit non-ProjectileHero:"+otherGo.name);
        }
    }
}
