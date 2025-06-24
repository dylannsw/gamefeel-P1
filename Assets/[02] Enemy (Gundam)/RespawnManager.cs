using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RespawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation, float delay)
    {
        StartCoroutine(DelayedSpawn(prefab, position, rotation, delay));
    }

    private IEnumerator DelayedSpawn(GameObject prefab, Vector3 position, Quaternion rotation, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (prefab != null)
            Instantiate(prefab, position, rotation);
    }
}

