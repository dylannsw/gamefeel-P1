using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ExampleHitscanRanged : Weapon
{
    [Header("Player Reference")]
    public PlayerController Player;

    [Header("Resource Management")]
    public int MaxAmmo = 50;
    public int CurrentAmmo = 50;
    public int LightAmmoCost = 1;
    public int MediumAmmoCost = 5;
    public int HeavyAmmoCost = 10;

    [Header("Attack Ranges")]
    public float LightAttackRange = 5f;
    public float MediumAttackRange = 8f;
    public float HeavyAttackRange = 10f;

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
            CurrentAttack = AttackType.Medium;
            MediumAttackEvent?.Invoke();
            StartCoroutine(PerformRangedAttack(0.5f));
        }
    }

    public override void PerformHeavyAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentAmmo >= HeavyAmmoCost)
        {
            CurrentAttack = AttackType.Heavy;
            HeavyAttackEvent?.Invoke();
            StartCoroutine(PerformRangedAttack(1f));
        }
    }

    public override void PerformReload()
    {
        if(!IsAttacking)
        {
            ReloadEvent?.Invoke();
            CurrentAmmo = MaxAmmo;
            UpdateHUD();
        }

        //Space for custom implementation
    }

    public IEnumerator PerformRangedAttack(float delay)
    {
        IsAttacking = true;

        yield return new WaitForSeconds(delay);

        switch (CurrentAttack)
        {
            case AttackType.Light:
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
                CurrentAmmo -= LightAmmoCost;
                break;

            case AttackType.Medium:
                if (Physics.Raycast(Player.PlayerCamera.transform.position,
                                   Player.PlayerCamera.transform.forward,
                                   out RaycastHit mediumHit,
                                   MediumAttackRange))
                {
                    Instantiate(MediumHitEffect, mediumHit.point, Quaternion.identity);
                    if (mediumHit.collider.gameObject.GetComponent<EnemyController>())
                    {
                        var enemy = mediumHit.collider.gameObject.GetComponent<EnemyController>();
                        enemy.ReceiveDamage(AttackType.Medium, MediumAttackDamage);
                    }
                }
                StartTimeDilation(MediumTimeDilation);
                CurrentAmmo -= MediumAmmoCost;
                break;

            case AttackType.Heavy:
                if (Physics.Raycast(Player.PlayerCamera.transform.position,
                                   Player.PlayerCamera.transform.forward,
                                   out RaycastHit heavyHit,
                                   HeavyAttackRange))
                {
                    Instantiate(HeavyHitEffect, heavyHit.point, Quaternion.identity);
                    if (heavyHit.collider.gameObject.GetComponent<EnemyController>())
                    {
                        var enemy = heavyHit.collider.gameObject.GetComponent<EnemyController>();
                        enemy.ReceiveDamage(AttackType.Heavy, HeavyAttackDamage);
                    }
                }
                StartTimeDilation(HeavyTimeDilation);
                CurrentAmmo -= HeavyAmmoCost;
                break;
        }

        UpdateHUD();
    }

    private void UpdateHUD()
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
}
