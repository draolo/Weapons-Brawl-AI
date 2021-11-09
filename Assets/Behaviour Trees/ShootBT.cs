using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBT : MonoBehaviour
{
    public float aiTime = .2f;
    public float beginWaitTime = 1f;
    private TargetAim targetAim;
    public Transform target;
    public LayerMask projectileObstacle;

    // Start is called before the first frame update
    void Start()
    {
        TargetAim targetAim = GetComponent<TargetAim>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool LineOfSight()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, projectileObstacle );
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    public bool TestAndSetStraight() {

        Vector2 direction = targetAim.Aim();
        if (direction.x <= -999)
        {
            return false;

        }
        targetAim.SetAim(direction);
        return true;
    }

    public bool EmptyFireLine()
    {
        // Vector2 impactPoint = targetAim.CollisionPredictionStupid();
        return false;
    }

    public bool TestAndSetLobbed()
    {

        Vector2 direction = targetAim.Aim(true);
        if (direction.x <= -999)
        {
            return false;

        }
        
        targetAim.SetAim(direction);
        return true;
    }

}
