using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ExampleProjectileRanged : Weapon
{
    [Header("Player Reference")]
    public PlayerController Player;

    [Header("Bullet References")]
    public GameObject Bullet;
    public float LightBulletSpeed = 5f;
    public float MediumBulletSpeed = 5f;
    public float HeavyBulletSpeed = 5f;

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

    [Header("Attack Delays")]
    public float InitialLightAttackDelay = 0f;
    public float InitialMediumAttackDelay = 0.833f;
    public float InitialHeavyAttackDelay = 1f;

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
            StartCoroutine(PerformRangedAttack(InitialLightAttackDelay));
        }
    }

    public override void PerformMediumAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentAmmo >= MediumAmmoCost)
        {
            CurrentAttack = AttackType.Medium;
            MediumAttackEvent?.Invoke();
            StartCoroutine(PerformRangedAttack(InitialMediumAttackDelay));
        }
    }

    public override void PerformHeavyAttack()
    {
        //Make sure we arent currently in an attack
        if (!IsAttacking && CurrentAmmo >= HeavyAmmoCost)
        {
            CurrentAttack = AttackType.Heavy;
            HeavyAttackEvent?.Invoke();
            StartCoroutine(PerformRangedAttack(InitialHeavyAttackDelay));
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

                GameObject spawnedLightBullet = Instantiate(Bullet, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                Instantiate(LightHitEffect, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                ExampleBullet bulletLightScript = spawnedLightBullet.GetComponent<ExampleBullet>();
                Rigidbody Lightrb = spawnedLightBullet.GetComponent<Rigidbody>();

                bulletLightScript.SetLifetime(LightAttackRange);
                bulletLightScript.SetBulletDamage(LightAttackDamage);
                
                Lightrb.velocity = Player.PlayerCamera.transform.TransformDirection(Vector3.forward * LightBulletSpeed);

                CurrentAmmo -= LightAmmoCost;
                break;

            case AttackType.Medium:

                GameObject spawnedMediumBullet = Instantiate(Bullet, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                Instantiate(MediumHitEffect, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                ExampleBullet bulletMediumScript = spawnedMediumBullet.GetComponent<ExampleBullet>();
                Rigidbody Mediumrb = spawnedMediumBullet.GetComponent<Rigidbody>();

                bulletMediumScript.SetLifetime(MediumAttackRange);
                bulletMediumScript.SetBulletDamage(MediumAttackDamage);
                Mediumrb.velocity = Player.PlayerCamera.transform.TransformDirection(Vector3.forward * MediumBulletSpeed);
                
                StartTimeDilation(MediumTimeDilation);

                yield return new WaitForSeconds(0.217f);

                GameObject spawnedMediumBullet2 = Instantiate(Bullet, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                Instantiate(MediumHitEffect, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                ExampleBullet bulletMediumScript2 = spawnedMediumBullet2.GetComponent<ExampleBullet>();
                Rigidbody Mediumrb2 = spawnedMediumBullet2.GetComponent<Rigidbody>();

                bulletMediumScript2.SetLifetime(MediumAttackRange);
                bulletMediumScript2.SetBulletDamage(MediumAttackDamage);
                Mediumrb2.velocity = Player.PlayerCamera.transform.TransformDirection(Vector3.forward * MediumBulletSpeed);

                CurrentAmmo -= MediumAmmoCost;
                break;

            case AttackType.Heavy:

                yield return new WaitForSeconds(1.766f);

                GameObject spawnedHeavyBullet = Instantiate(Bullet, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);
                ExampleBullet bulletHeavyScript = spawnedHeavyBullet.GetComponent<ExampleBullet>();
                Rigidbody Heavyrb = spawnedHeavyBullet.GetComponent<Rigidbody>();
                Instantiate(HeavyHitEffect, Player.PlayerCamera.transform.position + Player.PlayerCamera.transform.forward * 2, Player.PlayerCamera.transform.rotation);

                bulletHeavyScript.SetLifetime(HeavyAttackRange);
                bulletHeavyScript.SetBulletDamage(HeavyAttackDamage);

                Heavyrb.velocity = Player.PlayerCamera.transform.TransformDirection(Vector3.forward * HeavyBulletSpeed);

                spawnedHeavyBullet.transform.localScale = new Vector3(2f, 2f, 2f);

                StartTimeDilation(HeavyTimeDilation);
                CurrentAmmo -= HeavyAmmoCost;
                break;
        }

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        AmmoFill.fillAmount = (float)CurrentAmmo / MaxAmmo;
        AmmoText.text = $"{CurrentAmmo}\n/\n{MaxAmmo}";
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
