using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAP2D;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public SAP2DAgent agent = null;

    public Timer health = new Timer(2);

    public GameObject bulletFab = null;
    public float range = 10;
    public Timer attackRate = new Timer(2f);

    public Sprite[] enemySprites;

    [HideInInspector]
    public Vector2 prevPosition = new Vector2();
    [HideInInspector]
    public SpriteRenderer sp = null;

    public void Shoot()
    {
        if (!attackRate.IsComplete())
            return;

        Instantiate(bulletFab, transform.position, Quaternion.identity).GetComponent<Bullet>().Initialize((agent.Target.position - transform.position).normalized, false, 1);
    }
}
