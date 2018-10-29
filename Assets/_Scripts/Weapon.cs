﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType
{
    none, //the default / no weapon
    blaster, //a simple blaster
    spread, //two shots simultaneously
    phaser, //shot that move in waves [NI]
    missile, //homing missiles [NI]
    laser, //damage over time [NI]
    shield, //raise shieldLevel
}
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //the letter to show on the power-up
    public Color color = Color.white; //color of collar and power up
    public GameObject projectilePrefab; // prefab for projectile
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; //amount of damage caused
    public float continuousDamage = 0; //damage per second (laser)
    public float delayBetweenShots = 0;
    public float velocity = 20; //speed of projectiles
}

public class Weapon :MonoBehaviour {

    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")] [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; //Time last shot was fired
    private Renderer collarRend;
 


    // Use this for initialization
    void Start()
    {

        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        SetType(_type);

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        //find the fireDelegate of the parent
        GameObject rootGo = transform.root.gameObject;

        if (rootGo.GetComponent<Hero>() != null)
        {
            rootGo.GetComponent<Hero>().fireDelegate += Fire;

        }
    }
    public WeaponType type
    {

        get { return (_type); }
        set { SetType(value); }
    }
    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        Debug.Log("setting type"+gameObject.name+wt);
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        
        lastShotTime = 0; //you can always fire immediately after _type is set
    }

    public void Fire()
    {
        //if this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        //if it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;



            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                break;
        }
    }
        public Projectile MakeProjectile()
        {
            GameObject go = Instantiate< GameObject>(def.projectilePrefab);
            if (transform.parent.gameObject.tag == "Hero")
            {
                go.tag = "ProjectileHero";
                go.layer = LayerMask.NameToLayer("ProjectileHero");
            }
            else
            {
                go.tag = "ProjectileEnemy";
                go.layer = LayerMask.NameToLayer("ProjectileEnemy");
            }
            go.transform.position = collar.transform.position;
            go.transform.SetParent( PROJECTILE_ANCHOR,true);
            Projectile p = go.GetComponent<Projectile>();
            p.type = type;
            lastShotTime = Time.time;
            return (p);
        }

    


}
