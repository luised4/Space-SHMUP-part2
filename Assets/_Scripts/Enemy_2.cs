using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {
    [Header("Set in Inspector:Enemy_2")]
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;

    [Header("Set Dynamically:Enemy_2")]
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;
    // Use this for initialization
    void Start () {
        p0 = Vector3.zero;
        p0.x = bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight,bndCheck.camHeight);

        

        p1= Vector3.zero;
        //pick any point on the left side of the screen
        p1.x = bndCheck.camWidth +bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);
        

       

        //possibly swap sides
        if (Random.value < 0.5f)
        {
            //setting the .x of each point to its negative will move it to the other side of the screen
            p0.x *= -1;
            p1.x *= -1;
        }

        //set the birthTime to the current time
        birthTime = Time.time;


    }

    public override void Move()
    {
        //Bezier curves work based on a u value between 0 and 1
        float u = (Time.time - birthTime) / lifeTime;

        //if u > 1 then it has been longer than lifeTime since birthTime
        if (u > 1)
        {
            //this Enemy_2 has finished its life
            Destroy(this.gameObject);
            return;
        }

        //adjust u by adding an easing curve based on a Sine Wave
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //interpoalte the two linear interpolation points
        pos = (1 - u) * p0 + u * p1;
        base.Move();
    }
}
