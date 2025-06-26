using System.Collections;
using UnityEngine;

public class FunnelController_Medium : MonoBehaviour
{
    [Header("Targeting")]
    private Transform target;
    private Transform startPos;
    private Transform firePos;

    [Header("Timing Settings")]
    public float chargeTime = 6.0f;
    public float aimDuration = 0.15f;
    public float moveSpeed = 10f;
    public float moveDuration = 1f;
    public AnimationCurve moveEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float returnDelay = 4.2f;

    [Header("VFX")]
    public GameObject laserEffectPrefab;
    public GameObject chargingVFXPrefab;
    private GameObject activeChargeVFX;
    public GameObject hitVFXPrefab;
    public Transform firePoint;

    [Header("Damage Settings")]
    public float range = 100f;

    [Header("Beam Duration Damage")]
    public float fireDuration = 4.2f;
    public float tickInterval = 0.2f;
    public float damagePerTick = 3f;

    private FunnelManager manager;

    public void Initialize(Transform start, Transform fire, Transform enemyTarget, FunnelManager mgr)
    {
        startPos = start;
        firePos = fire;
        target = enemyTarget;
        manager = mgr;

        transform.position = start.position;
        StartCoroutine(FunnelRoutine());
    }

    private IEnumerator FunnelRoutine()
    {
        // Move to fire position
        yield return StartCoroutine(MoveToPoint(firePos));
        AudioManager.Instance.Play("FUNNEL");

        // Rotate to face target
        yield return StartCoroutine(FaceTarget());

        // Wait and charge
        Debug.Log($"{gameObject.name} charging for {chargeTime}s...");
        yield return StartCoroutine(PlayChargingVFX());

        yield return StartCoroutine(LaserTickCoroutine());

        yield return new WaitForSeconds(returnDelay);

        //Return to start position
        yield return StartCoroutine(MoveToPoint(startPos));

        Destroy(gameObject);
        manager.OnFunnelAttackFinished();
    }

    private IEnumerator MoveToPoint(Transform destination)
    {
        if (destination == null) yield break;

        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float easedT = moveEase.Evaluate(t);
            transform.position = Vector3.Lerp(start, destination.position, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination.position;
    }

    private IEnumerator FaceTarget()
    {
        if (target == null) yield break;

        Vector3 toTarget = target.position - transform.position;
        if (toTarget == Vector3.zero) yield break;

        Quaternion targetRot = Quaternion.LookRotation(toTarget);
        float elapsed = 0f;

        while (elapsed < aimDuration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, elapsed / aimDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
    }

    private IEnumerator LaserTickCoroutine()
    {
        if (laserEffectPrefab != null && firePoint != null)
        {
            GameObject laser = Instantiate(laserEffectPrefab, firePoint.position, firePoint.rotation, firePoint);
            Destroy(laser, fireDuration);
        }

        float elapsed = 0f;

        while (elapsed < fireDuration)
        {
            RaycastHit hit;
            Vector3 origin = firePoint.position;
            Vector3 direction = firePoint.forward;

            if (Physics.Raycast(origin, direction, out hit, range))
            {
                var enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.ReceiveDamage(AttackType.Medium, damagePerTick);
                    Debug.Log("Tick damage dealt");
                }

                GameObject vfx = Instantiate(hitVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(vfx, 2f);
            }

            Debug.DrawRay(origin, direction * range, Color.magenta, tickInterval);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Debug.Log("Laser beam finished ticking");
    }

    private IEnumerator PlayChargingVFX()
    {
        if (chargingVFXPrefab != null && firePoint != null)
        {
            activeChargeVFX = Instantiate(chargingVFXPrefab, firePoint.position, firePoint.rotation, firePoint);
        }

        yield return new WaitForSeconds(chargeTime);

        // Stop or destroy charge VFX
        if (activeChargeVFX != null)
        {
            Destroy(activeChargeVFX);
        }
    }
}
