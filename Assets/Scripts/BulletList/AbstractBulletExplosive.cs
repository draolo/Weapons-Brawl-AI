using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractBulletExplosive : NetworkBehaviour {

    [Range(2, 60)] [SerializeField] public int ExplosionRadius = 2;

    public GameObject shootedBy;
    public int BulletPower = 20;
    public int FlingIntensity = 10;
    public float speed = 20f;

    public Rigidbody2D rb;
    public MapController map;
    public GameObject explosionEffect;


    protected void Start()
    {
        map = FindObjectOfType<MapController>();
    }


    public void ExplodeCircle()
    {
        if (isServer)
        {
            DestroyMapCircle();
            DestroyWalls();
            DamageWhoIsInsideTheExplosion();
            FlingWhoIsInsideTheExplosionCallback();
            ExplosionAnimationCallback();
        }
    }


    [Server]
    private void DestroyMapCircle()
    {
        foreach (var p in new BoundsInt(-ExplosionRadius, -ExplosionRadius, 0, 2 * ExplosionRadius + 1, 2 * ExplosionRadius + 1, 1).allPositionsWithin)
        {
            int x = p[0];
            int y = p[1];
            if (x * x + y * y - ExplosionRadius * ExplosionRadius < 0)
            {
                Vector3Int position = Vector3Int.FloorToInt(gameObject.transform.position);
                position.z = 0; // A volte diventa -1 a caso quindi lo forzo a 0 io
                Vector3Int destroyPos = position + p;
                map.CmdDestroyTile(destroyPos.x, destroyPos.y, destroyPos.z);
            }
        }
    }





    [Server]
    void FlingWhoIsInsideTheExplosionCallback()
    {
        //FlingWhoIsInsideTheExplosion();
        RpcFlingWhoIsInsideTheExplosion(); //rpc are called also on the server
    }

    [ClientRpc]
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




    [Server]
    void ExplosionAnimationCallback()
    {
        //ExplosionAnimation();
        RpcExplosionAnimation(); //rpc are called also on the server
    }




    [ClientRpc]
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




    [Server]
    void DamageWhoIsInsideTheExplosion()
    {
        Collider2D[] hittedList = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        foreach (Collider2D hitted in hittedList)
        {
            if (hitted.CompareTag("Player"))
            {
                hitted.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(BulletPower, shootedBy);
            }
        }
    }

    [Server]
    void DestroyWalls()
    {
        Collider2D[] hittedList = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        foreach (Collider2D hitted in hittedList)
        {
            if (hitted.CompareTag("Wall"))
            {
                NetworkServer.UnSpawn(hitted.gameObject);
                Destroy(hitted.gameObject);
            }
        }
    }




    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
