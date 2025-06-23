using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunnelManager : MonoBehaviour
{
    public GameObject funnelPrefab;
    public Transform[] pathPoints; // All available positions
    public Transform enemy; // Assign in inspector or via tag lookup
    public Transform[] funnelLaunchPoints; // Each funnel's spawn point

    public int numberOfFunnels = 3;
    public int pointsPerFunnel = 3;

    public void ActivateFunnels()
    {
        for (int i = 0; i < numberOfFunnels; i++)
        {
            GameObject funnel = Instantiate(funnelPrefab, funnelLaunchPoints[i].position, funnelLaunchPoints[i].rotation);
            FunnelController controller = funnel.GetComponent<FunnelController>();
            List<Transform> selectedPoints = pathPoints.OrderBy(x => Random.value).Take(pointsPerFunnel).ToList();
            controller.Initialize(funnelLaunchPoints[i], selectedPoints, enemy);
        }
    }
}

