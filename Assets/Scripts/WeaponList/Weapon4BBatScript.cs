using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon4BBatScript : AbstractWeaponGeneric
{
    public PlayerAnimationController AnimationController;

    public float attackRange = 5;
    public int damagePower = 20;
    public LayerMask PlayerLayer;

    public float FlingIntensity = 10;

    public override void Attack(int charge)
    {
        if (AnimationController == null)
        {
            AnimationController = Player.GetComponent<PlayerAnimationController>();
            AnimationController.BBatAnim = this.GetComponent<Animator>();
        }
        
        AnimationController.PlayBBatAnimation();


        RaycastHit2D[] hits = Physics2D.RaycastAll(Player.transform.position, firePoint.right, attackRange, PlayerLayer);

        foreach (RaycastHit2D hit in hits)
        {
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            if (enemy && enemy.gameObject != Player.gameObject)
            {
                enemy.CmdTakeDamage(damagePower, Player.gameObject);


                Vector3 direction = (firePoint.right + Vector3.up) * FlingIntensity;
                enemy.gameObject.GetComponent<PlayerManager>().CmdSetVelocity(direction.x, direction.y);
            }
        }
    }
}
