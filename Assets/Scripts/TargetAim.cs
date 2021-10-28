using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAim : MonoBehaviour
{
    public Transform target;
    public Transform firePoint;
    public int speed=50;

    public Vector2 Aim(bool lobbed=false)
    {

        Vector2 g = Physics.gravity;
        float gravity = g.magnitude;
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
                if (!lobbed)
                {
                    ttt = Mathf.Min(time0, time1);
                }
                else
                {
                    ttt = Mathf.Max(time0, time1);
                }
            }
        }
        Vector2 aimTo= (2 * deltaVec - g* ttt*ttt) / (2 * speed * ttt); //# return the firing vector
        return aimTo.normalized;
    }



    public float GetHeightAtTime(Vector2 angle, float time)
    {
        return speed * time * angle.y + 0.5f * Physics.gravity.y * time * time + transform.position.y;
    }

    public float GetLengthAtTime(Vector2 angle, float time)
    {
        return speed * time * angle.x + transform.position.x;
    }

    public Vector2 CollisionPredictionStupid(Vector2 angle,int halfOfpoints=3)
    {
        float maxT = Mathf.Abs((speed * angle.y) / (Physics.gravity.y));
        Vector2 maxHeight = new Vector2(GetLengthAtTime(angle, maxT), GetHeightAtTime(angle, maxT));
        float step = maxT / halfOfpoints;
        Vector2 beginPos = firePoint.position;
        Vector2 endPos;
        RaycastHit2D hit;
        for (int i = 1; i <= halfOfpoints * 2; i++)
        {
            endPos =new Vector2(GetLengthAtTime(angle, step*i), GetHeightAtTime(angle, step*i));
            float xDistancePoint = endPos.x - firePoint.position.x;
            float xDistanceTarget = target.position.x - firePoint.position.x;
            if (Mathf.Abs(xDistanceTarget) < Mathf.Abs(xDistancePoint))
            {
                hit = Physics2D.Linecast(beginPos, target.position, 1 << LayerMask.NameToLayer("Ground"));
                if (hit.collider != null)
                {
                    return hit.point;
                }
                return new Vector2(-9999, -9999);
            }
            else
            {
                hit = Physics2D.Linecast(beginPos, endPos, 1 << LayerMask.NameToLayer("Ground"));
                if (hit.collider != null)
                {
                    return hit.point;
                }
            }
            beginPos = endPos;
        }
        hit = Physics2D.Linecast(beginPos, target.position, 1 << LayerMask.NameToLayer("Ground"));
        if (hit.collider != null)
        {
            return hit.point;
        }

        return new Vector2(-9999,-9999);
    }

    void OnDrawGizmos()
    {
        int halfOfpoints = 3;
        Gizmos.color = Color.green;
        Vector2 angle = Aim();
        float maxT = Mathf.Abs((speed * angle.y) / (Physics.gravity.y));
        Vector2 maxHeight = new Vector2(GetLengthAtTime(angle, maxT), GetHeightAtTime(angle, maxT));
        float step = maxT / halfOfpoints;
        Vector2 beginPos = firePoint.position;
        Vector2 endPos;
        for (int i = 1; i <= halfOfpoints * 2; i++)
        {
            endPos = new Vector2(GetLengthAtTime(angle, step * i), GetHeightAtTime(angle, step * i));
            float xDistancePoint = endPos.x - firePoint.position.x;
            float xDistanceTarget = target.position.x - firePoint.position.x;
            if (Mathf.Abs(xDistanceTarget) < Mathf.Abs(xDistancePoint))
            {
                Gizmos.DrawLine(beginPos, target.position);
                return;
            }
            else
            {
                Gizmos.DrawLine(beginPos, endPos);
                beginPos = endPos;
            }
        }
        Gizmos.DrawLine(beginPos, target.position);
    }
}
