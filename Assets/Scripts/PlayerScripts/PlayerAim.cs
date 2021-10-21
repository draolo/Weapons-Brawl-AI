using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAim : NetworkBehaviour
{

    public float firePointRadius = 3.15f;
    public float speed = 10;

    private float upperAngle = 85f;
    private float lowerAngle = 275f;

    public GameObject FirePointPivot;

    void Update()
    {
        if (hasAuthority)
        {
            var rot = FirePointPivot.transform.rotation.eulerAngles.z;
            var dir = Input.GetAxisRaw("Vertical");
            var a = rot < upperAngle + 10f & dir < 0;
            var b = rot > lowerAngle - 10f & dir > 0;
            if (rot < upperAngle || rot > lowerAngle || a || b)
                FirePointPivot.transform.Rotate(0f, 0f, dir * speed);
        }





    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, firePointRadius);
    }
}
