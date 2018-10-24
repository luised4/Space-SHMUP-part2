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
    private BoundCheck bndCheck;


    void Awake()
    {
        S = this;
        //Set utils.camBounds
        bndCheck=GetComponent<BoundCheck>();

       
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);

       
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


}
