using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using static UnityEngine.EventSystems.EventTrigger;

public class ExampleHitscanRanged : Weapon
{
    [Header("Player Reference")]
    public PlayerController Player;
    public float attackSensitivity = 1f;
    public CameraShake cameraShake;
    public CameraRecoil cameraRecoil;

    [Header("Camera FOV")]
    public float defaultFOV;
    public float heavyAttackFOV = 40f;
    public float fovTransitionSpeed = 0.5f;
    public Coroutine fovCoroutine;

    [Header("Resource Management")]
    public int MaxAmmo = 50;
    public int CurrentAmmo = 50;
    public int LightAmmoCost = 1;
    public int MediumAmmoCost = 5;
    public int HeavyAmmoCost = 10;

    [Header("Attack Ranges")]
    public float LightAttackRange = 50f;
    public float MediumAttackRange = 50f;
    public float HeavyAttackRange = 50f;

    [Header("Attack Damages")]
    public float LightAttackDamage = 5f;
    public float MediumAttackDamage = 10f;
    public float HeavyAttackDamage = 20f;

    [Header("Feedback Parameters")]
    public GameObject LightHitEffect;
    public GameObject MediumHitEffect;
    public GameObject HeavyHitEffect;
    public float MediumTimeDilation = 0.5f;
    public float HeavyTimeDilation = 0.3f;
    public float TimeDilationDuration = 0.5f;
    public float TimeRestoreSpeed = 2f;
    Coroutine timeDilationCoroutine;

    [Header("Heavy Attack")]
    public ParticleSystem HeavyBeamVFXCharge;
    public ParticleSystem HeavyBeanVFXHold;
    public ParticleSystem HeavyBeamVFXFire;
    public VisualEffect HeavyBeamVFX;
    public Transform HeavyBeamSpawnPoint;
    private Coroutine heavyBeamCoroutine;
    public float heavyBeamTickRate = 0.3f;

    [Header("Medium Attack")]
    public ParticleSystem MedBeamVFXCharge;
    public ParticleSystem MedBeamVFXFire;
    public VisualEffect MedBeamVFX;
    public Transform MedBeamSpawnPoint;
    public Transform MedChargeSpawnPoint;

    [Header("Light Attack")]
    public ParticleSystem LightBeamVFXFire;
    public VisualEffect LightMuzzleVFX;
    public Transform LightBeamSpawnPoint;


    [Header("HUD References")]
    public GameObject AmmoHUD;
    public Image AmmoFill;
    public TextMeshProUGUI AmmoText;

    private void Awake()
    {
        //Make a reference to our PlayerController
        Player = FindObjectOfType<PlayerController>();

        //Set up our controls to match those referenced
        InitializeControls(Player);

        Cursor.lockState = CursorLockMode.Locked;

        //Set default FOV based on current camera FOV
        defaultFOV = Player.PlayerCamera.fieldOfView;

        //Camera Shake Reference
        cameraShake = Player.PlayerCamera.transform.parent.GetComponent<CameraShake>();

        //Camera Recoil Reference
        cameraRecoil = Player.PlayerCamera.transform.parent.GetComponent<CameraRecoil>();


    }
    public override void OnWeaponEquip()
    {
        EquipEvent?.Invoke();
    }

    public override void PerformLightAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentAmmo >= LightAmmoCost)
        {
            IsAttacking = true;
            CurrentAttack = AttackType.Light;
            LightAttackEvent?.Invoke();
            StartCoroutine(PerformRangedAttack(0.05f));
        }
    }

    public override void PerformMediumAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentAmmo >= MediumAmmoCost)
        {
            IsAttacking = true;
            CurrentAttack = AttackType.Medium;
            MediumAttackEvent?.Invoke();
            StartCoroutine(PerformRangedAttack(2f));
        }
    }

    public override void PerformHeavyAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentAmmo >= HeavyAmmoCost)
        {
            IsAttacking = true;
            CurrentAttack = AttackType.Heavy;
            HeavyAttackEvent?.Invoke();

            StartCoroutine(PerformRangedAttack(10f));

            if (fovCoroutine != null) StopCoroutine(fovCoroutine);

            fovCoroutine = StartCoroutine(ChangeFOV(heavyAttackFOV));
        }
    }

    public override void PerformReload()
    {
        if (!IsAttacking)
        {
            IsAttacking = true;
            ReloadEvent?.Invoke();
            CurrentAmmo = MaxAmmo;
            UpdateHUD();
        }

        //Space for custom implementation
        StartCoroutine(EndReloadLock());
    }

    public IEnumerator PerformRangedAttack(float delay)
    {
        IsAttacking = true;

        yield return new WaitForSeconds(delay);

        switch (CurrentAttack)
        {
            case AttackType.Light:

                Debug.DrawRay(Player.PlayerCamera.transform.position, Player.PlayerCamera.transform.forward * LightAttackRange, Color.red, 2f);

                if (Physics.Raycast(Player.PlayerCamera.transform.position,
                                   Player.PlayerCamera.transform.forward,
                                   out RaycastHit lightHit,
                                   LightAttackRange))
                {
                    Instantiate(LightHitEffect, lightHit.point, Quaternion.identity);
                    if (lightHit.collider.gameObject.GetComponent<EnemyController>())
                    {
                        var enemy = lightHit.collider.gameObject.GetComponent<EnemyController>();
                        enemy.ReceiveDamage(AttackType.Light, LightAttackDamage);
                    }
                }

                if (cameraRecoil != null) cameraRecoil.FireRecoil(CameraRecoil.RecoilStrength.Light);

                CurrentAmmo -= LightAmmoCost;
                break;

            case AttackType.Medium:
                // if (Physics.Raycast(Player.PlayerCamera.transform.position,
                //                    Player.PlayerCamera.transform.forward,
                //                    out RaycastHit mediumHit,
                //                    MediumAttackRange))
                // {
                //     Instantiate(MediumHitEffect, mediumHit.point, Quaternion.identity);
                //     if (mediumHit.collider.gameObject.GetComponent<EnemyController>())
                //     {
                //         var enemy = mediumHit.collider.gameObject.GetComponent<EnemyController>();
                //         enemy.ReceiveDamage(AttackType.Medium, MediumAttackDamage);
                //     }
                // }

                //if (cameraRecoil != null) cameraRecoil.FireRecoil(CameraRecoil.RecoilStrength.Medium);
                //StartTimeDilation(MediumTimeDilation);
                //CurrentAmmo -= MediumAmmoCost;

                break;

            case AttackType.Heavy:
                // if (Physics.Raycast(Player.PlayerCamera.transform.position,
                //                    Player.PlayerCamera.transform.forward,
                //                    out RaycastHit heavyHit,
                //                    HeavyAttackRange))
                // {
                //     Instantiate(HeavyHitEffect, heavyHit.point, Quaternion.identity);
                //     if (heavyHit.collider.gameObject.GetComponent<EnemyController>())
                //     {
                //         var enemy = heavyHit.collider.gameObject.GetComponent<EnemyController>();
                //         enemy.ReceiveDamage(AttackType.Heavy, HeavyAttackDamage);
                //     }
                // }

                //if (cameraRecoil != null) cameraRecoil.FireRecoil(CameraRecoil.RecoilStrength.Heavy);
                //StartTimeDilation(HeavyTimeDilation);
                //CurrentAmmo -= HeavyAmmoCost;

                break;
        }

        UpdateHUD();
        ResetAttackState();
    }

    public void UpdateHUD()
    {
        AmmoFill.fillAmount = (float)CurrentAmmo / MaxAmmo;
        AmmoText.text = $"{CurrentAmmo}/{MaxAmmo}";
    }

    private void StartTimeDilation(float dilationAmount)
    {
        if (timeDilationCoroutine != null)
        {
            StopCoroutine(timeDilationCoroutine);
        }
        timeDilationCoroutine = StartCoroutine(TimeDilationCoroutine(dilationAmount));
    }

    private IEnumerator TimeDilationCoroutine(float dilationAmount)
    {
        // Slow down time
        Time.timeScale = dilationAmount;

        // Wait for the dilation duration
        yield return new WaitForSecondsRealtime(TimeDilationDuration);

        // Gradually restore time to normal
        while (Time.timeScale < 1)
        {
            Time.timeScale = Mathf.Min(Time.timeScale + (Time.unscaledDeltaTime * TimeRestoreSpeed), 1);
            yield return null;
        }

        // Ensure time is fully restored
        Time.timeScale = 1;
    }

    public override void OnWeaponUnEquip()
    {
        //Space for custom implementation
    }

    public IEnumerator ChangeFOV(float targetFOV)
    {
        float startFOV = defaultFOV;
        float t = 0;

        while (MathF.Abs(Player.PlayerCamera.fieldOfView - targetFOV) > 0.1f)
        {
            t += Time.deltaTime * fovTransitionSpeed;
            Player.PlayerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        Player.PlayerCamera.fieldOfView = targetFOV;
    }

    private void ResetAttackState()
    {
        IsAttacking = false;
        CurrentAttack = AttackType.None;
    }

    private IEnumerator EndReloadLock()
    {
        yield return new WaitForSeconds(3.4f); // Adjust for reload animation time
        ResetAttackState();
    }

    public void FireRaycast()
    {
        RaycastHit hit;
        GameObject effect = null;
        float range = 0;
        float damage = 0;

        switch (CurrentAttack)
        {
            case AttackType.Medium:
                range = MediumAttackRange;
                damage = MediumAttackDamage;
                effect = MediumHitEffect;
                break;
            case AttackType.Heavy:
                range = HeavyAttackRange;
                damage = HeavyAttackDamage;
                effect = HeavyHitEffect;
                break;
            default:
                return;
        }

        Debug.DrawRay(Player.PlayerCamera.transform.position,
                      Player.PlayerCamera.transform.forward * range, Color.yellow, 2f);

        if (Physics.Raycast(Player.PlayerCamera.transform.position,
                            Player.PlayerCamera.transform.forward,
                            out hit, range))
        {
            if (effect != null)
                Instantiate(effect, hit.point, Quaternion.identity);

            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.ReceiveDamage(CurrentAttack, damage);
        }

        CurrentAmmo -= GetAmmoCost(CurrentAttack);
        UpdateHUD();
    }

    private int GetAmmoCost(AttackType attack)
    {
        return attack switch
        {
            AttackType.Light => LightAmmoCost,
            AttackType.Medium => MediumAmmoCost,
            AttackType.Heavy => HeavyAmmoCost,
            _ => 0
        };
    }

    //Heavy Attack Tick Damage
    public void StartHeavyBeamTick()
    {
        if (heavyBeamCoroutine != null) return;
        heavyBeamCoroutine = StartCoroutine(HeavyBeamTick());
    }

    public void StopHeavyBeamTick()
    {
        if (heavyBeamCoroutine != null)
        {
            StopCoroutine(heavyBeamCoroutine);
            heavyBeamCoroutine = null;
        }
    }

    private IEnumerator HeavyBeamTick()
    {
        while (true)
        {
            if (Physics.Raycast(Player.PlayerCamera.transform.position,
                                Player.PlayerCamera.transform.forward,
                                out RaycastHit hit, HeavyAttackRange))
            {
                var enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.ReceiveDamage(AttackType.Heavy, HeavyAttackDamage);
                }

                if (HeavyHitEffect != null)
                    Instantiate(HeavyHitEffect, hit.point, Quaternion.identity);
            }

            yield return new WaitForSeconds(heavyBeamTickRate);
        }
    }


}
