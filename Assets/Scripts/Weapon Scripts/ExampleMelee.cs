using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleMelee : Weapon
{
    [Header("Player Reference")]
    public PlayerController Player;

    [Header("Resource Management")]
    public float MaxStamina = 100f;
    public float CurrentStamina = 100f;
    public float StaminaRechargeRate = 2.5f;
    public float LightStaminaCost = 10f;
    public float MediumStaminaCost = 20f;
    public float HeavyStaminaCost = 30f;

    [Header("Attack Damages")]
    public float LightAttackDamage = 5f;
    public float MediumAttackDamage = 10f;
    public float HeavyAttackDamage = 20f;

    [Header("Feedback Parameters")]
    public float LightHitStun = 0.05f;
    public float MediumHitStun = 0.1f;
    public float HeavyHitStun = 0.25f;
    public GameObject LightHitEffect; //Delete these and use the appropriate code in Weapon.cs
    public GameObject MediumHitEffect; //if you choose to use per-object tag VFX spawning
    public GameObject HeavyHitEffect;

    [Header("HUD References")]
    public GameObject StaminaHUD;
    public Image StaminaFill;

    private void Awake()
    {
        //Make a reference to our PlayerController
        Player = FindObjectOfType<PlayerController>();

        //Set up our controls to match those referenced
        InitializeControls(Player);
    }

    private void Update()
    {
        HandleResources();
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        StaminaFill.fillAmount = (float)CurrentStamina / MaxStamina;
    }

    private void HandleResources()
    {
        if(CurrentStamina < MaxStamina)
            CurrentStamina += Time.deltaTime * StaminaRechargeRate;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
    }

    public override void OnWeaponEquip()
    {
        //Space for custom implementation
    }

    public override void PerformLightAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentStamina >= LightStaminaCost)
        {
            //Set our current attack mode (for referencing events on other objects)
            CurrentAttack = AttackType.Light;
            CurrentStamina -= LightStaminaCost;

            //Invoke our light attack events
            LightAttackEvent?.Invoke();

            //Space for custom implementation
        }
    }

    public override void PerformMediumAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentStamina >= MediumStaminaCost)
        {
            //Set our current attack mode (for referencing events on other objects)
            CurrentAttack = AttackType.Medium;
            CurrentStamina -= MediumStaminaCost;

            //Invoke our light attack events
            MediumAttackEvent?.Invoke();

            //Space for custom implementation
        }
    }

    public override void PerformHeavyAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentStamina >= HeavyStaminaCost)
        {
            //Set our current attack mode (for referencing events on other objects)
            CurrentAttack = AttackType.Heavy;
            CurrentStamina -= HeavyStaminaCost;

            //Invoke our light attack events
            HeavyAttackEvent?.Invoke();

            //Space for custom implementation
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If we're currently attacking
        if (IsAttacking)
        {
            //Spawn hit effects
            switch (CurrentAttack)
            {
                case AttackType.Light:
                    if(LightHitEffect != null)
                        Instantiate(LightHitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
                    break;

                case AttackType.Medium:
                    if(MediumHitEffect != null)
                        Instantiate(MediumHitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
                    break;

                case AttackType.Heavy:
                    if(HeavyHitEffect != null)
                        Instantiate(HeavyHitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
                    break;
            }

            //If the object hit has an EnemyController
            if (other.GetComponent<EnemyController>() != null)
            {
                EnemyController enemy = other.GetComponent<EnemyController>();
                Debug.Log("Enemy Hit");
                switch (CurrentAttack)
                {
                    case AttackType.Light:
                        enemy.ReceiveDamage(AttackType.Light, LightAttackDamage);
                        StartCoroutine(ApplyHitStun(LightHitStun));
                        break;

                    case AttackType.Medium:
                        enemy.ReceiveDamage(AttackType.Medium, MediumAttackDamage);
                        StartCoroutine(ApplyHitStun(MediumHitStun));
                        break;

                    case AttackType.Heavy:
                        enemy.ReceiveDamage(AttackType.Heavy, HeavyAttackDamage);
                        StartCoroutine(ApplyHitStun(HeavyHitStun));
                        break;
                }
            }
        }
    }

    IEnumerator ApplyHitStun(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    public override void OnWeaponUnEquip()
    {
        //Space for custom implementation
    }
}
