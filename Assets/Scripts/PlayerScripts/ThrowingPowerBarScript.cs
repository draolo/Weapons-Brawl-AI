using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingPowerBarScript : MonoBehaviour {

    private Transform barSprite;

    [Range(1, 20)] [SerializeField] public int chargeSpeed = 1;
    public int Charge;

    private Quaternion parentRotation;

    private void Awake()
    {
        barSprite = transform.Find("Bar/BarSprite");
        parentRotation = transform.parent.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = parentRotation;
    }

    public void SetSize(int size)
    {
        barSprite.localScale = new Vector3(size * 1f / 100, 1f);
    }

    private void Update()
    {
        Charge = Mathf.FloorToInt(chargeSpeed * 10 * Time.fixedTime) % 100;
        SetSize(Charge);
    }

}
