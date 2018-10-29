using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    

    [Header("Set in Inspector Enemy")]


    public float speed = 10f; //the speed in m/s
    public float fireRate = 0.3f; //seconds/shot (Unused)
    public float health = 10;
    public int score = 100; //points earned for destroying this
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;
    [Header("These fields are set Dynamically")]
    public Color[] OriginalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedDestruction = false;

    public BoundCheck bndCheck;



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
        materials = Utils.GetAllMaterials(gameObject);
        OriginalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            OriginalColors[i] = materials[i].color;
        }
    }




    // Update is called once per frame
    void Update () {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();

        }
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

        switch (otherGo.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGo.GetComponent<Projectile>();

                ShowDamage();
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGo);
                    break;
                }

                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    if (!notifiedDestruction)

                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedDestruction = true;
                    Destroy(this.gameObject);
                }

                Destroy(otherGo);
                break;

           // default: print("Enemy hit by non-ProjectileHero:" + otherGo.name);
        
    }
}
    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = OriginalColors[i];
        }
        showingDamage = false;

    }
}
