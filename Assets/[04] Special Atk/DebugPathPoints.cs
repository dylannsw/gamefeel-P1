using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPathPoints : MonoBehaviour
{
    public Color gizmoColor = Color.red;
    public float gizmoRadius = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        foreach (Transform child in transform)
        {
            if (child != null) Gizmos.DrawWireSphere(child.position, gizmoRadius);
        }  
    }
}
