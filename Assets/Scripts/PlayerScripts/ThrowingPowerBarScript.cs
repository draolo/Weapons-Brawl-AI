using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingPowerBarScript : MonoBehaviour
{
    private Transform barSprite;

    [Range(1, 20)] [SerializeField] public int chargeSpeed = 1;
    public float charge;

    private Quaternion parentRotation;

    private void Awake()
    {
        barSprite = transform.Find("Bar/BarSprite");
        parentRotation = transform.parent.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = parentRotation;
    }

    public void SetSize(int size)
    {
        barSprite.localScale = new Vector3(size * 1f / 100, 1f);
    }

    public int GetCharge()
    {
        return Mathf.FloorToInt(charge);
    }

    private void Update()
    {
        SetSize(GetCharge());
    }

    private void FixedUpdate()
    {
        charge = (charge + (chargeSpeed * 10 * Time.fixedDeltaTime)) % 100;
    }
}