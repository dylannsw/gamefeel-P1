using System.Collections;
using System.Collections.Generic;
using FullOpaqueVFX;
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
    public GameObject syncFireLaserEffectPrefab; //Final beam
    public GameObject finalChargeVFXPrefab; //Final charge VFX
    public GameObject[] finalHitVFXPrefabs; //For final beam only
    public float finalChargeDuration = 1.5f;
    public GameObject muzzleEffectPrefab;
    public Transform firePoint;
    public GameObject[] hitVFXPrefabs;

    [Header("Damage Settings")]
    public float damage = 5f;
    public float range = 100f;

    [Header("Tick Damage Settings")]
    public float tickDamage = 1f;
    public float tickRate = 0.5f;
    public float tickDuration = 3f;

    private FunnelManager manager;
    private Transform finalPhasePoint;
    private bool isFinalPhase = false;

    public void Initialize(Transform start, List<Transform> points, Transform enemyTarget, FunnelManager mgr, Transform finalPoint)
    {
        startPoint = start;
        travelPoints = points;
        target = enemyTarget;
        manager = mgr;
        finalPhasePoint = finalPoint;

        transform.position = start.position;
        StartCoroutine(ExecutePath());
    }

    private IEnumerator ExecutePath()
    {
        // Regular travel path
        foreach (Transform point in travelPoints)
        {
            AudioManager.Instance.Play("FUNNEL");
            yield return StartCoroutine(MoveToPoint(point));
            yield return StartCoroutine(FaceTargetAndFire());
            yield return new WaitForSeconds(waitTime);
        }

        // Move to the specific final phase point (already assigned by FunnelManager)
        if (finalPhasePoint != null)
        {
            yield return StartCoroutine(MoveToPoint(finalPhasePoint));
        }

        // Notify manager that this funnel is ready for the final synced laser
        manager.RegisterReadyFunnel(this);
    }


    public void FireFinalLaserAndReturn()
    {
        StartCoroutine(FinalSequence());
    }

    private IEnumerator FinalSequence()
    {
        isFinalPhase = true;

        yield return StartCoroutine(FaceTarget());

        // Play charging effect
        yield return StartCoroutine(PlayFinalChargeVFX());

        // Fire final synced laser
        StartCoroutine(FinalLaserTickRoutine());
        yield return new WaitForSeconds(tickDuration); // Wait while tick runs

        yield return StartCoroutine(MoveToPoint(startPoint));

        Destroy(gameObject);
        manager.OnFunnelAttackFinished();
    }

    private IEnumerator PlayFinalChargeVFX()
    {
        if (finalChargeVFXPrefab != null && firePoint != null)
        {
            AudioManager.Instance.Play("HEAVYSPECIAL");
            
            GameObject charge = Instantiate(finalChargeVFXPrefab, firePoint.position, firePoint.rotation, firePoint);
            yield return new WaitForSeconds(finalChargeDuration);
            Destroy(charge);
        }
        else
        {
            yield return new WaitForSeconds(finalChargeDuration);
        }
    }

    private IEnumerator MoveToPoint(Transform point)
    {
        AudioManager.Instance.Play("FUNNEL");
        while (Vector3.Distance(transform.position, point.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, point.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FaceTarget()
    {
        if (target == null) yield break;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = lookRotation;
    }

    private IEnumerator FaceTargetAndFire()
    {
        yield return StartCoroutine(FaceTarget());
        FireLaser();
    }

    private void FireLaser(GameObject laserPrefabOverride = null)
    {
        GameObject laserVFX = laserPrefabOverride ?? laserEffectPrefab;

        if (laserVFX != null && firePoint != null)
        {
            GameObject vfx = Instantiate(laserVFX, firePoint.position, firePoint.rotation);
            Destroy(vfx, 6.2f);
        }

        if (muzzleEffectPrefab != null)
        {
            GameObject vfx = Instantiate(muzzleEffectPrefab, firePoint.position, firePoint.rotation);
            Destroy(vfx, 2f);
        }

        AudioManager.Instance.PlayRandomFromGroup("HEAVYSPECIAL");

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range))
        {
            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(AttackType.Heavy, damage);
            }

            if (hitVFXPrefabs.Length > 0)
            {
                GameObject vfx = Instantiate(hitVFXPrefabs[Random.Range(0, hitVFXPrefabs.Length)], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(vfx, 2f);
            }
        }

        Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.yellow, 1f);
    }

    private IEnumerator FinalLaserTickRoutine()
    {
        float elapsed = 0f;

        // Optionally spawn a persistent VFX here instead of multiple instantiates
        GameObject beamVFX = null;
        if (syncFireLaserEffectPrefab != null && firePoint != null)
            beamVFX = Instantiate(syncFireLaserEffectPrefab, firePoint.position, firePoint.rotation);

        while (elapsed < tickDuration)
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range))
            {
                var enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                    enemy.ReceiveDamage(AttackType.Heavy, tickDamage);

                if (finalHitVFXPrefabs.Length > 0)
                {
                    GameObject vfx = Instantiate(finalHitVFXPrefabs[Random.Range(0, finalHitVFXPrefabs.Length)], hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(vfx, 2.5f);
                }
            }

            Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 0.5f);
            elapsed += tickRate;
            yield return new WaitForSeconds(tickRate);
        }

        if (beamVFX != null) Destroy(beamVFX);
    }
}
