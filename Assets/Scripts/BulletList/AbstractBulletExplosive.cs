﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBulletExplosive : MonoBehaviour
{
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
        Vector3 position = gameObject.transform.position;
        map.DestroyCircle(position, ExplosionRadius);
    }

    private void FlingWhoIsInsideTheExplosionCallback()
    {
        //FlingWhoIsInsideTheExplosion();
        RpcFlingWhoIsInsideTheExplosion(); //rpc are called also on the server
    }

    private void RpcFlingWhoIsInsideTheExplosion()
    {
        FlingWhoIsInsideTheExplosion();
    }

    private void FlingWhoIsInsideTheExplosion()
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

    private void ExplosionAnimationCallback()
    {
        //ExplosionAnimation();
        RpcExplosionAnimation(); //rpc are called also on the server
    }

    private void RpcExplosionAnimation()
    {
        ExplosionAnimation();
    }

    private void ExplosionAnimation()
    {
        explosionEffect.transform.localScale = new Vector3(ExplosionRadius, ExplosionRadius, 0);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void DamageWhoIsInsideTheExplosion()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}