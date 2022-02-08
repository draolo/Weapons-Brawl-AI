using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAim : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject FirePointPivot;
    [SerializeField] private LayerMask projectileObstacle;
    [SerializeField] private int maxSpeed = 50;

    public float firePointDistance;

    private Vector2 lastTest;

    private void Awake()
    {
        firePointDistance = Vector2.Distance(firePoint.position, FirePointPivot.transform.position);
    }

    public Vector2 GetShootingAngle(float normalizedPower, bool lobbed = false)
    {
        if (target == null)
        {
            return new Vector2(-9999, -9999);
        }
        float speed = maxSpeed * normalizedPower;
        Vector2 g = Physics.gravity;
        float gravity = g.magnitude;
        Vector2 deltaVec = target.position - transform.position;// calculate vector from target to start
        float delta = deltaVec.magnitude;
        float a = gravity * gravity;
        float b = -4 * (Vector2.Dot(g, deltaVec) + speed * speed);
        float c = 4 * delta * delta;
        if (4 * a * c > b * b)
        {
            return new Vector2(-9999, -9999); // check for no real solutions
        }
        float time0 = Mathf.Sqrt((-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a)); // find candidate times
        float time1 = Mathf.Sqrt((-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a));
        float ttt = 0;
        if (time0 < 0)
        {  //find the time to target
            if (time1 < 0)
            {
                return new Vector2(-9999, -9999);
            }
            else
            {
                ttt = time1;
            }
        }
        else
        {
            if (time1 < 0)
            {
                ttt = time0;
            }
            else
            {
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
        Vector2 aimTo = (2 * deltaVec - g * ttt * ttt) / (2 * speed * ttt); //# return the firing vector
        return aimTo.normalized;
    }

    public float GetPower(Vector2 angle)
    {
        angle.Normalize();
        Vector2 g = Physics.gravity;
        float gravity = g.magnitude;
        Vector2 deltaVec = target.position - transform.position;// calculate vector from target to start
        deltaVec.x = Math.Abs(deltaVec.x);
        float gxx = gravity * deltaVec.x * deltaVec.x;
        float denom = 2f * angle.x * ((angle.y * deltaVec.x) - (angle.x * deltaVec.y));
        if (denom == 0)
        {
            return 99;
        }
        else
        {
            float power = Mathf.Sqrt(gxx / denom);
            return power / maxSpeed;
        }
    }

    private float GetHeightAtTime(Vector2 angle, float time, float speed)
    {
        return speed * time * angle.y + 0.5f * Physics.gravity.y * time * time + transform.position.y;
    }

    private float GetLengthAtTime(Vector2 angle, float time, float speed)
    {
        return speed * time * angle.x + transform.position.x;
    }

    public float GetPredictedTimeOfImpact(Vector2 angle, float normalizedPower, Vector2 target)
    {
        //easy formula could lead to unpredicted situation for negative target y
        angle.Normalize();
        float v = maxSpeed * normalizedPower;
        Vector2 gravity = Physics.gravity;
        float g = gravity.magnitude;
        float sin = angle.y;
        float deltaY = target.y - transform.position.y;
        float a = g / 2;
        float b = -v * sin;
        float c = deltaY;
        if (4 * a * c > b * b)
        {
            return -1;
        }
        float time0 = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a); // find candidate times
        float time1 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        float deltat0 = Mathf.Abs(target.x - GetLengthAtTime(angle, time0, v));
        float deltat1 = Mathf.Abs(target.x - GetLengthAtTime(angle, time1, v));
        if (deltat0 < deltat1)
        {
            return time0;
        }
        else
        {
            return time1;
        }
    }

    public RaycastHit2D CollisionPredictionStupid(Vector2 angle, float normalizedPower, int halfOfpoints = 3)
    {
        lastTest = angle;
        RaycastHit2D mock = new RaycastHit2D();
        mock.point = new Vector2(-9999, -9999);
        float speed = maxSpeed * normalizedPower;
        float maxT = Mathf.Abs((speed * angle.y) / (Physics.gravity.y));
        Vector2 maxHeight = new Vector2(GetLengthAtTime(angle, maxT, speed), GetHeightAtTime(angle, maxT, speed));
        float step = maxT / halfOfpoints;

        Vector2 beginPos = (Vector2)transform.position + (firePointDistance * angle);
        Vector2 endPos;
        RaycastHit2D hit;
        for (int i = 1; i < halfOfpoints * 2; i++)
        {
            endPos = new Vector2(GetLengthAtTime(angle, step * i, speed), GetHeightAtTime(angle, step * i, speed));
            float xDistancePoint = endPos.x - beginPos.x;
            float xDistanceTarget = target.position.x - beginPos.x;
            if (Mathf.Abs(xDistanceTarget) < Mathf.Abs(xDistancePoint))
            {
                hit = Physics2D.Linecast(beginPos, target.position, projectileObstacle);
                if (hit.collider != null)
                {
                    return hit;
                }
                return mock;
            }
            else
            {
                hit = Physics2D.Linecast(beginPos, endPos, projectileObstacle);
                if (hit.collider != null)
                {
                    return hit;
                }
            }
            beginPos = endPos;
        }
        hit = Physics2D.Linecast(beginPos, target.position, projectileObstacle);
        if (hit.collider != null)
        {
            return hit;
        }

        return mock;
    }

    public void SetAim(Vector2 direction)
    {
        if (direction.x < -999)
        {
            return;
        }
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);

        FirePointPivot.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private void OnDrawGizmos()
    {
        if (target == null)
        {
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawSphere((Vector2)transform.position + (firePointDistance * lastTest), 1);
        int halfOfpoints = 3;
        Gizmos.color = Color.green;
        Vector2 angle = GetShootingAngle(1, false);
        float speed = maxSpeed;
        float maxT = Mathf.Abs((speed * angle.y) / (Physics.gravity.y));
        Vector2 maxHeight = new Vector2(GetLengthAtTime(angle, maxT, speed), GetHeightAtTime(angle, maxT, speed));
        float step = maxT / halfOfpoints;
        Vector2 beginPos = firePoint.position;
        Vector2 endPos;
        for (int i = 1; i <= halfOfpoints * 2; i++)
        {
            endPos = new Vector2(GetLengthAtTime(angle, step * i, speed), GetHeightAtTime(angle, step * i, speed));
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

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}