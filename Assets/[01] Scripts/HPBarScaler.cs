using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarScaler : MonoBehaviour
{
    public float maxDistance = 2f;
    public float minScale = 0.75f;
    public float maxScale = 1f;

    private Camera mainCamera;
    private Vector3 originalScale;

    void Start()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        float scale = Mathf.Clamp(distance / maxDistance, minScale, maxScale);
        transform.localScale = originalScale * scale;
    }
}
