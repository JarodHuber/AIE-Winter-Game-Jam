﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAP2D;

public class Enemy : MonoBehaviour
{
    SAP2DAgent agent = null;

    public GameObject bulletFab = null;
    public float range = 10;
    public Timer attackRate = new Timer(.5f);

    private void Awake()
    {
        agent = GetComponent<SAP2DAgent>();
    }

    private void Update()
    {
        if (!attackRate.IsComplete(false))
            attackRate.CountByTime();

        if (Vector2.Distance(transform.position, agent.Target.position) > range)
            return;

        agent.CanMove = Vector2.Distance(transform.position, agent.Target.position) < range / 2;

        Shoot();
    }

    void Shoot()
    {
        if (!attackRate.IsComplete())
            return;

        // Attack
    }
}
