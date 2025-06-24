using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker
{
    public static Transform CurrentEnemy { get; private set; }

    public static void Register(Transform enemy)
    {
        CurrentEnemy = enemy;
    }

    public static void Clear()
    {
        CurrentEnemy = null;
    }
}

