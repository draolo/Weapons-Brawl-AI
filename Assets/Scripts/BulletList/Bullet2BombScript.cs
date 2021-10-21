using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Bullet2BombScript : AbstractBulletExplosive
{
    public float ExplosionDelay = 3f;

    new void Start()
    {
        base.Start();
        rb.velocity = transform.right * speed;
        StartCoroutine(ExplodeWithDelay(ExplosionDelay));
    }




    IEnumerator ExplodeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExplodeCircle();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isServer)
        {
            var enemy = collision.gameObject.GetComponent<PlayerHealth>();
            if (enemy)
            {
                float vel = GetComponent<Rigidbody2D>().velocity.magnitude;
                if (vel > 5)
                    enemy.CmdTakeDamage(Mathf.CeilToInt(vel / 3), shootedBy);
            }
        }
    }


  
}
