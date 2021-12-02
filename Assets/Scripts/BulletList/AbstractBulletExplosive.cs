using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBulletExplosive : MonoBehaviour {

    [Range(2, 60)] [SerializeField] public int ExplosionRadius = 2;

    public GameObject shootedBy;
    public int BulletPower = 20;
    public int FlingIntensity = 10;
    public float speed = 20f;

    public Rigidbody2D rb;
    public Map map;
    public GameObject explosionEffect;


    protected void Start()
    {
        map = FindObjectOfType<Map>();
    }


    public void ExplodeCircle()
    {

        {
            DestroyMapCircle();
            DamageWhoIsInsideTheExplosion();
            FlingWhoIsInsideTheExplosionCallback();
            ExplosionAnimationCallback();
        }
    }


    private void DestroyMapCircle()
    {
        foreach (var p in new BoundsInt(-ExplosionRadius, -ExplosionRadius, 0, 2 * ExplosionRadius + 1, 2 * ExplosionRadius + 1, 1).allPositionsWithin)
        {
            int x = p[0];
            int y = p[1];
            if (x * x + y * y - ExplosionRadius * ExplosionRadius < 0)
            {
                Vector3 position = gameObject.transform.position;
                position.z = 0; // A volte diventa -1 a caso quindi lo forzo a 0 io
                Vector3 destroyPos = position + p;
                int destroyX, destroyY;
                map.GetMapTileAtPoint(destroyPos, out destroyX, out destroyY);
                map.SetTile(destroyX,destroyY,TileType.Empty);
            }
        }
    }





    void FlingWhoIsInsideTheExplosionCallback()
    {
        //FlingWhoIsInsideTheExplosion();
        RpcFlingWhoIsInsideTheExplosion(); //rpc are called also on the server
    }

    void RpcFlingWhoIsInsideTheExplosion()
    {
        FlingWhoIsInsideTheExplosion();
    }

    void FlingWhoIsInsideTheExplosion()
    {
        Collider2D[] hittedList = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        foreach (Collider2D hitted in hittedList)
        {
            if (hitted.CompareTag("Player"))
            {
                Vector3 heading = hitted.transform.position - transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;
                hitted.gameObject.GetComponent<Rigidbody2D>().velocity = direction * FlingIntensity;
            }
        }
    }




    void ExplosionAnimationCallback()
    {
        //ExplosionAnimation();
        RpcExplosionAnimation(); //rpc are called also on the server
    }




    void RpcExplosionAnimation()
    {
        ExplosionAnimation();
    }




    private void ExplosionAnimation()
    {
        explosionEffect.transform.localScale = new Vector3(ExplosionRadius, ExplosionRadius, 0);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }



    void DamageWhoIsInsideTheExplosion()
    {
        Collider2D[] hittedList = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        foreach (Collider2D hitted in hittedList)
        {
            if (hitted.CompareTag("Player"))
            {
                hitted.gameObject.GetComponent<PlayerHealth>().TakeDamage(BulletPower, shootedBy);
            }
        }
    }







    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
