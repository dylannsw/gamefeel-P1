using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelController_Heavy : MonoBehaviour
{
    private Transform startPoint;
    private List<Transform> travelPoints;
    private Transform target;
    public float moveSpeed = 10f;
    public float waitTime = 0.5f;

    [Header("VFX")]
    public GameObject laserEffectPrefab;
    public GameObject muzzleEffectPrefab;
    public Transform firePoint;

    [Header("Damage Settings")]
    public float damage = 5f;
    public float range = 100f;

    public void Initialize(Transform start, List<Transform> points, Transform enemyTarget)
    {
        startPoint = start;
        travelPoints = points;
        target = enemyTarget;
        StartCoroutine(ExecutePath());
    }

    private IEnumerator ExecutePath()
    {
        foreach (Transform point in travelPoints)
        {
            yield return StartCoroutine(MoveToPoint(point));
            yield return StartCoroutine(FaceTargetAndFire());
            yield return new WaitForSeconds(waitTime);
        }

        // Return to start
        yield return StartCoroutine(MoveToPoint(startPoint));
        Destroy(gameObject); // Or pool it
    }

    private IEnumerator MoveToPoint(Transform point)
    {
        while (Vector3.Distance(transform.position, point.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, point.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FaceTargetAndFire()
    {
        if (target == null) yield break;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

        // Fire logic
        FireLaser();
        yield return null;
    }

    private void FireLaser()
    {
        if (laserEffectPrefab != null && firePoint != null)
        {
            GameObject vfx = Instantiate(laserEffectPrefab, firePoint.position, firePoint.rotation);
            Destroy(vfx, 1f);

            GameObject muzzleVFX = Instantiate(muzzleEffectPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleVFX, 1f);
        }

        RaycastHit hit;
        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.forward;

        if (Physics.Raycast(origin, direction, out hit, range))
        {
            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(AttackType.Heavy, damage);
            }
        }

        Debug.DrawRay(origin, direction * range, Color.cyan, 1f);
        Debug.Log($"{gameObject.name} heavy special fired at enemy!");

    }
}
