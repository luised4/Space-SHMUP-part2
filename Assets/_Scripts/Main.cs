using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
    static public Main S;
    

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f; // the # enemies/second
    public float enemyDefaultPadding = 1.5f; //Padding for position
    public WeaponDefinition[] weaponDefinitions;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster,WeaponType.blaster,WeaponType.spread,WeaponType.shield
    };
    private BoundCheck bndCheck;

    public void ShipDestroyed(Enemy e)
    {
        //potentially generate a PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            //random.value generates a value between 0 and 1 though never exactly 1
            //if the e.powerUpDropChance is 0.50f, a powerup will be generated 50% of the time. For testing it's 1f

            //choose shich powerup to pick
            //pick one from teh possibilites in powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            //spawn a powerup
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //set it to the proper WeaponType
            pu.SetType(puType);

            //set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    void Awake()
    {
        S = this;
        //Set utils.camBounds
        bndCheck=GetComponent<BoundCheck>();

       
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }

    }

    public void SpawnEnemy()
    {
        //pick a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);


        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundCheck>().radius);

        }

        Vector3 pos = Vector3.zero;

        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        //call SpawnEnemy() again in a couple of seconds
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
    }
    public void DelayRestart(float delay)
    {
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
      
        return (new WeaponDefinition());
    }

   

}
