using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public static RespawnManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RespawnEnemy(Vector3 position, Quaternion rotation, float delay)
    {
        StartCoroutine(DelayedSpawn(position, rotation, delay));
    }

    private IEnumerator DelayedSpawn(Vector3 position, Quaternion rotation, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemyPrefab != null)
            Instantiate(enemyPrefab, position, rotation);
    }
}

