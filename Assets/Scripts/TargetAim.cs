using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAim : MonoBehaviour
{
    public Transform target;
    public int speed=50;

    public Vector2 Aim()
    {

        Vector2 g = Physics.gravity;
        float gravity = g.magnitude;
        Debug.Log(gravity);
        Vector2 deltaVec =   target.position- transform.position;// calculate vector from target to start
        float delta = deltaVec.magnitude;
        float a = gravity * gravity;
        float b = -4 * ( Vector2.Dot(g,deltaVec) + speed * speed);
        float c = 4 * delta * delta;
        if (4 * a * c > b * b) {
            return new Vector2(-9999, -9999); // check for no real solutions
        }
        float time0 = Mathf.Sqrt((-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a)); // find candidate times
        float time1 = Mathf.Sqrt((-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a));
        float ttt=0;
        if (time0 < 0) {  //find the time to target
            if (time1 < 0)
            {
                return new Vector2(-9999,-9999);
            }
            else {
                ttt = time1;
            }
        }
        else {
            if (time1 < 0) {
                ttt = time0;
            }
            else {
                ttt = Mathf.Min(time0, time1);
            }
        }
        return (2 * deltaVec - g* ttt*ttt) / (2 * speed * ttt); //# return the firing vector
    }
}
