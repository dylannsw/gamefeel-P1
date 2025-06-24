using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunnelManager : MonoBehaviour
{
    public enum FunnelAttackMode { Light, Medium, Heavy}

    [Header("General Reference")]
    public Transform enemy;
    public int funnelMode = 0;
    public ExampleHitscanRanged weapon;

    [Header("Light Special Attack")]
    public Transform lightFunnelParent;
    public GameObject lightFunnelPrefab;
    public Transform[] lightStartPoints;
    public Transform[] lightFirePoints;
    public int lightShots = 3;
    public float lightShotInterval = 0.4f;

    [Header("Medium Special Attack")]
    public Transform mediumFunnelParent;
    public GameObject mediumFunnelPrefab;
    public Transform[] mediumStartPoints;
    public Transform[] mediumFirePoints;


    [Header("Heavy Special Attack")]
    public GameObject heavyFunnelPrefab;
    public Transform[] pathPoints;
    public Transform[] funnelLaunchPoints;
    public int numberOfFunnels = 3;
    public int pointsPerFunnel = 3;

    public void ActivateFunnels(FunnelAttackMode mode)
    {
        switch (mode)
        {
            case FunnelAttackMode.Light:
                ActivateLightFunnels();
                break;
            case FunnelAttackMode.Medium:
                ActivateMediumFunnels();
                break;
            case FunnelAttackMode.Heavy:
                ActivateHeavyFunnels();
                break;
        }
        
    }

    public void ActivateLightFunnels()
    {
        enemy = EnemyTracker.CurrentEnemy;
        if (enemy == null) return;

        for (int i = 0; i < Mathf.Min(lightStartPoints.Length, lightFirePoints.Length); i++)
        {
            GameObject funnel = Instantiate(lightFunnelPrefab, lightStartPoints[i].position, lightStartPoints[i].rotation, lightFunnelParent);
            FunnelController_Light controller = funnel.GetComponent<FunnelController_Light>();
            controller.Initialize(lightStartPoints[i], lightFirePoints[i], enemy, lightShots, lightShotInterval);
        }
    }

    public void ActivateMediumFunnels()
    {
        enemy = EnemyTracker.CurrentEnemy;
        if (enemy == null) return;

        for (int i = 0; i < Mathf.Min(mediumStartPoints.Length, mediumFirePoints.Length); i++)
        {
            GameObject funnel = Instantiate(mediumFunnelPrefab, mediumStartPoints[i].position, mediumStartPoints[i].rotation, mediumFunnelParent);
            FunnelController_Medium controller = funnel.GetComponent<FunnelController_Medium>();
            controller.Initialize(mediumStartPoints[i], mediumFirePoints[i], enemy);
        }

        weapon.PerformHeavyAttack();

    }

    public void ActivateHeavyFunnels()
    {
        enemy = EnemyTracker.CurrentEnemy;
        if (enemy == null) return;

        for (int i = 0; i < numberOfFunnels; i++)
        {
            GameObject funnel = Instantiate(heavyFunnelPrefab, funnelLaunchPoints[i].position, funnelLaunchPoints[i].rotation);
            FunnelController_Heavy controller = funnel.GetComponent<FunnelController_Heavy>();
            List<Transform> selectedPoints = pathPoints.OrderBy(x => Random.value).Take(pointsPerFunnel).ToList();
            controller.Initialize(funnelLaunchPoints[i], selectedPoints, enemy);
        }
    }
}

