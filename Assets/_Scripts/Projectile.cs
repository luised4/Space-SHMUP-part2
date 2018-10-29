using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    private BoundCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField] // a
    private WeaponType _type;

    // This public property masks the field _type and takes action when it is set
    public WeaponType type
    { //c
        get
        {
            return (_type);
        }
        set
        {
            SetType(value); // c
        }
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundCheck>();
        rend = GetComponent<Renderer>(); //d
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (bndCheck.offUp)
        {                                        // a
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the _type private field and colors this projectile to match the
    /// WeaponDefinition.
    /// </summary>
    /// <param name="eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType)
    { // e
      //Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}