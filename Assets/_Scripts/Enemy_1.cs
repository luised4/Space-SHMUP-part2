using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector Enemy_1")]
    public float waveFrequency = 2;
    //sine wave width in meters
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; //the initial x value of pos
    private float birthTime;

    // Use this for initialization
    void Start () {

        x0 = pos.x;

        birthTime = Time.time;

    }
    public override void Move()
    {
        //because pos is a property you can't directly set pos.x so get the pos as an editable Vector3
        Vector3 tempPos = pos;
        //theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        //base.Move() still handles the movement down in y
        base.Move();
        //print(bndCheck.isOnScreen);
    }
    // Update is called once per frame
   
}
