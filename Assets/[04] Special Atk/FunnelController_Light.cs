using System;
using System.Collections;
using UnityEngine;

public class FunnelController_Light : MonoBehaviour
{
    [Header("Targeting")]
    private Transform target;
    private Transform startPos;
    private Transform firePos;

    [Header("Firing Parameters")]
    private int numberOfShots = 3;
    private float intervalBetweenShots = 0.5f;
    private float aimDuration = 0.1f;
    public float moveSpeed = 10f;

    [Header("Movement")]
    public float moveDuration = 1f;
    public AnimationCurve moveEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("VFX")]
    public GameObject laserEffectPrefab;
    public GameObject muzzleEffectPrefab;
    public GameObject hitVFXPrefab;
    public Transform firePoint;

    [Header("Damage Settings")]
    public float damage = 5f;
    public float range = 100f;

    private FunnelManager manager;

    public void Initialize(Transform start, Transform fire, Transform enemyTarget, int shots, float interval, FunnelManager mgr)
    {
        startPos = start;
        firePos = fire;
        target = enemyTarget;
        numberOfShots = shots;
        intervalBetweenShots = interval;
        manager = mgr;

        transform.position = start.position;
        StartCoroutine(FunnelRoutine());
    }

    private IEnumerator FunnelRoutine()
    {
        // Move to fire position
        yield return StartCoroutine(MoveToPoint(firePos));

        // Fire loop
        for (int i = 0; i < numberOfShots; i++)
        {
            yield return StartCoroutine(FaceTarget());
            FireLaser();
            yield return new WaitForSeconds(intervalBetweenShots);
        }

        // Return to start position
        yield return StartCoroutine(MoveToPoint(startPos));

        Destroy(gameObject);
        manager.OnFunnelAttackFinished();
    }

    private IEnumerator MoveToPoint(Transform destination)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float easedT = moveEase.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, destination.position, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination.position;
    }
    private IEnumerator FaceTarget()
    {
        if (target == null) yield break;

        yield return StartCoroutine(FaceDirection(target.position));
    }

    private IEnumerator FaceDirection(Vector3 worldTarget)
    {
        Quaternion targetRot = Quaternion.LookRotation(worldTarget - transform.position);
        float elapsed = 0f;

        while (elapsed < aimDuration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, elapsed / aimDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
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
                enemy.ReceiveDamage(AttackType.Light, damage);
            }

            if (hitVFXPrefab != null)
            {
                GameObject vfxInstance = Instantiate(hitVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(vfxInstance, 1f);
            }

        }

        Debug.DrawRay(origin, direction * range, Color.cyan, 1f);
        Debug.Log($"{gameObject.name} light special fired at enemy!");
    }
}
