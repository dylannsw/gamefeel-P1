using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunnelManager : MonoBehaviour
{
    public enum FunnelAttackMode { Light, Medium, Heavy }

    [Header("General Reference")]
    public Transform enemy;
    public int funnelMode = 0;
    public ExampleHitscanRanged weapon;
    public Transform playerTransform;
    private bool isFunnelAttackActive = false;

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
    public List<Transform> finalPhasePoints = new List<Transform>();
    private List<Transform> assignedFinalPoints = new List<Transform>(); // Temp tracking
    private List<FunnelController_Heavy> heavyReadyFunnels = new List<FunnelController_Heavy>();
    private int totalHeavyFunnelsExpected = 0;

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
            controller.Initialize(lightStartPoints[i], lightFirePoints[i], enemy, lightShots, lightShotInterval, this);
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
            controller.Initialize(mediumStartPoints[i], mediumFirePoints[i], enemy, this);
        }

        // Triggers heavy ranged attack together with medium funnel
        weapon.PerformHeavyAttack();
    }

    public void ActivateHeavyFunnels()
    {
        enemy = EnemyTracker.CurrentEnemy;
        if (enemy == null) return;

        heavyReadyFunnels.Clear();
        totalHeavyFunnelsExpected = numberOfFunnels;

        assignedFinalPoints.Clear();

        for (int i = 0; i < numberOfFunnels; i++)
        {
            if (i >= funnelLaunchPoints.Length || i >= finalPhasePoints.Count)
            {
                Debug.LogWarning("Not enough launch or final points!");
                break;
            }

            GameObject funnel = Instantiate(heavyFunnelPrefab, funnelLaunchPoints[i].position, funnelLaunchPoints[i].rotation, playerTransform);
            FunnelController_Heavy controller = funnel.GetComponent<FunnelController_Heavy>();

            List<Transform> selectedPath = pathPoints.OrderBy(x => Random.value).Take(pointsPerFunnel).ToList();

            Transform uniqueFinalPoint = finalPhasePoints[i]; // Guarantee unique point per funnel
            assignedFinalPoints.Add(uniqueFinalPoint);

            controller.Initialize(funnelLaunchPoints[i], selectedPath, enemy, this, uniqueFinalPoint); // Pass unique final point
        }
    }

    public void RegisterReadyFunnel(FunnelController_Heavy funnel)
    {
        if (!heavyReadyFunnels.Contains(funnel))
            heavyReadyFunnels.Add(funnel);

        if (heavyReadyFunnels.Count >= totalHeavyFunnelsExpected)
        {
            // All funnels reached final point, fire final beam in sync
            foreach (var f in heavyReadyFunnels)
                f.FireFinalLaserAndReturn();

            heavyReadyFunnels.Clear(); // Reset after sync shot
        }
    }

    public void OnFunnelAttackFinished()
    {
        isFunnelAttackActive = false;
    }

    public void OnFunnelAttackStart()
    {
        isFunnelAttackActive = true;
    }

    public bool FunnelAttackState()
    {
        if (isFunnelAttackActive) return true;
        else return false;
    }

}
