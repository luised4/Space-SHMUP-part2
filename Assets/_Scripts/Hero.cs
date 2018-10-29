using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
    static public Hero S;
    // Singleton                                    // a

    [Header("Set in the Unity Inspector")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("These fields are set dynamically")]
    [SerializeField]
    public float _shieldLevel = 1;
    private GameObject lastTriggerGo = null;

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //a

    // Create a WeaponFireDeleage field named fireDelegate.
    public WeaponFireDelegate fireDelegate;

    void Start()
    {
        if (S == null)
        {
            S = this;  // Set the Singleton                                        // a
        }
        //		fireDelegate += TempFire;

        // Reset the ewapons to start _Hero with 1 blaster
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    void Update()
    {
        // Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");                             // b
        float yAxis = Input.GetAxis("Vertical");                               // b

        // Change transform.position basec on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic                        // c
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        // Allow the ship to fire
        //		if (Input.GetKeyDown (KeyCode.Space)) {
        //			TempFire ();
        //		}

        // Use the fireDelegate to fire Weapons
        // First, make sure the button is pressed: Axis("Jump")
        // Then ensure that fireDelegate isn't null to avoid an error
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        { //d
            fireDelegate(); // e
        }
    }

    //	void TempFire ()
    //	{                                                   // b
    //		GameObject projGO = Instantiate<GameObject> (projectilePrefab);
    //		projGO.transform.position = transform.position;
    //		Rigidbody rigidB = projGO.GetComponent<Rigidbody> ();
    //		rigidB.velocity = Vector3.up * projectileSpeed;
    //	}

    void OnTriggerEnter(Collider other)
    {
        //		print ("Triggered: " + other.gameObject.name); //it triggers when hits other's children but not other itself
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //		print ("Triggered: " + go.name);

        // Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo)
        {                                      // c
            return;
        }
        lastTriggerGo = go;                                             // d

        if (go.tag == "Enemy")
        {  // If the shield was triggered by an enemy
            shieldLevel--;        // Decrease the level of the shield by 1
            Destroy(go);          // ... and Destroy the enemy            // e
        }
        else if (go.tag == "PowerUp")
        {
            // If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);                  // f
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield: //a
                shieldLevel++;
                break;
            default: //b
                if (pu.type == weapons[0].type)
                { // If it is the same type //c
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        // Set it to pu.type
                        w.SetType(pu.type);
                    }
                }
                else
                { // If this is a different weapon type //d
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayRestart(gameRestartDelay);
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}