using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAimAuto : MonoBehaviour
{

    public float firePointRadius = 3.15f;
    public float speed = 10;

    private float upperAngle = 85f;
    private float lowerAngle = 275f;

    public GameObject FirePointPivot;

    void Update()
    {
        TargetAim targetAim= GetComponent<TargetAim>();
        Vector2 direction = targetAim.Aim();
        Debug.Log(direction);
        if (direction.x < -999)
        {
            return;
        }
        float angle = Vector2.Angle( Vector2.right, direction);
        
        FirePointPivot.transform.eulerAngles=new Vector3(0f, 0f, angle);

    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, firePointRadius);
    }
}
