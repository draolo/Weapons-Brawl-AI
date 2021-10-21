using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScript : MonoBehaviour {

    private Transform barSprite;
    private GameObject healthText;

    private Quaternion parentRotation;


    private void Awake()
    {
        barSprite = transform.Find("Bar/BarSprite");
        healthText = transform.Find("HPText").gameObject;
        healthText.GetComponent<MeshRenderer>().sortingOrder = 100;
        parentRotation = transform.parent.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = parentRotation;
    }

    public void SetHealth(int health)
    {
        healthText.GetComponent<TextMesh>().text = health.ToString();


        barSprite.localScale = new Vector3(health * 1f / 100, 1f);
        if(health <= 30)
            barSprite.Find("whiteBar").gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        else
            barSprite.Find("whiteBar").gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    
}
