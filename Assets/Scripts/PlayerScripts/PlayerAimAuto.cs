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







    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, firePointRadius);
    }
}
