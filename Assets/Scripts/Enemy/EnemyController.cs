using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Aiming")]
    public float turnSpeed = 5f;

    [Header("Positioning")]
    public float distance = 100f;
    public float verticalOffset = -15f;

    [Header("Health Parameters")]
    public float MaxHealth = 10;
    public float CurrentHealth = 10;

    [Header("Combat Parameter")]
    public int LightAttackDamage = 1;
    public int MediumAttackDamage = 2;
    public int HeavyAttackDamage = 3;

    [Header("Object References")]
    public Animator EnemyAnimator;
    public GameObject EnemyPrefab;
    public Image HPBarFill;
    public GameObject SpawnVFX;

    [Header("Event Systems")]
    public UnityEvent OnSpawn;
    public UnityEvent OnAttackLight;
    public UnityEvent OnAttackMedium;
    public UnityEvent OnAttackHeavy;
    public UnityEvent OnDamagedLight;
    public UnityEvent OnDamagedMedium;
    public UnityEvent OnDamagedHeavy;
    public UnityEvent OnDeath;

    [HideInInspector]
    private EnemyControls EC;
    private EnemyControls.BasicActions Controls;
    public float PitchVariation = 0.05f;
    public PlayerController Player;

    private void Awake()
    {
        OnSpawn?.Invoke();
        CurrentHealth = MaxHealth;
        UpdateHUD();
        Player = FindObjectOfType<PlayerController>();
        EC = new EnemyControls();
    }

    private void Update()
    {
        TrackPlayer();
        MaintainDistance();
        HandleManualAnimationInput();

        if(Input.GetKey(KeyCode.G)) EnemyAnimator.Play("MoveBack");

    }

    private void OnEnable()
    {
        EC.Enable();

        //Assign and reference our methods to our controls from the IM
        Controls = EC.Basic;
        Controls.EnemyLightAttack.performed += ctx => PerformLightAttack();
        Controls.EnemyMediumAttack.performed += ctx => PerformMediumAttack();
        Controls.EnemyHeavyAttack.performed += ctx => PerformHeavyAttack();
    }

    private void PerformLightAttack()
    {
        OnAttackLight?.Invoke();
        Player.TakeDamage(LightAttackDamage, AttackType.Light);
    }

    private void PerformMediumAttack()
    {
        OnAttackMedium?.Invoke();
        Player.TakeDamage(MediumAttackDamage, AttackType.Medium);

    }

    private void PerformHeavyAttack()
    {
        OnAttackHeavy?.Invoke();
        Player.TakeDamage(HeavyAttackDamage, AttackType.Heavy);
    }

    public void ReceiveDamage(AttackType strength, float damage)
    {
        CurrentHealth -= damage;
        UpdateHUD();

        if (CurrentHealth <= 0)
            EnemyDeath();
        else
            switch (strength)
            {
                case AttackType.Light:
                    OnDamagedLight?.Invoke();
                    //Space for custom implementation
                    break;

                case AttackType.Medium:
                    OnDamagedMedium?.Invoke();
                    //Space for custom implementation
                    break;

                case AttackType.Heavy:
                    OnDamagedHeavy?.Invoke();
                    //Space for custom implementation
                    break;
            }
    }

    private void EnemyDeath()
    {
        OnDeath?.Invoke();
        StartCoroutine(RespawnEnemy(6.5f));
    }

    IEnumerator RespawnEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
        Instantiate(SpawnVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);

    }
    public void PlayInterruptableAnimation(string stateName)
    {
        EnemyAnimator.Play(stateName, -1, 0);
    }

    private void UpdateHUD()
    {
        HPBarFill.fillAmount = (float)CurrentHealth / MaxHealth;
    }

    public void PlaySoundWithRandomPitch(AudioClip clip)
    {
        GetComponent<AudioSource>().pitch = 1 + UnityEngine.Random.Range(-PitchVariation, PitchVariation);
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    public void TrackPlayer()
    {
        if (Player == null) return;

        Vector3 direction = Player.transform.position - transform.position;
        direction.y = 0f; // Ignore vertical rotation

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    public void MaintainDistance()
    {
        if (Player == null) return;

        //Horizontal direction only (XZ)
        Vector3 horizontalDirection = (transform.position - Player.transform.position);
        horizontalDirection.y = 0f;
        horizontalDirection.Normalize();

        //Calculate target position with vertical offset
        Vector3 targetPosition = Player.transform.position + horizontalDirection * distance + Vector3.up * verticalOffset;

        //Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
    }

    private bool isBackMoving = false;
    private bool isFrontMoving = false;

    private void HandleManualAnimationInput()
    {
        bool backPressed = Input.GetKey(KeyCode.W);
        bool frontPressed = Input.GetKey(KeyCode.S);

        if (backPressed && !isBackMoving)
        {
            EnemyAnimator.ResetTrigger("BackToIdle");
            EnemyAnimator.SetTrigger("MoveBack");
            isBackMoving = true;
        }
        else if (!backPressed && isBackMoving)
        {
            EnemyAnimator.ResetTrigger("MoveBack");
            EnemyAnimator.SetTrigger("BackToIdle");
            isBackMoving = false;
        }

        if (frontPressed & !isFrontMoving)
        {
            EnemyAnimator.ResetTrigger("FrontToIdle");
            EnemyAnimator.SetTrigger("MoveFront");
            isFrontMoving = true;
        }
        else if (!frontPressed && isFrontMoving)
        {
            EnemyAnimator.ResetTrigger("MoveFront");
            EnemyAnimator.SetTrigger("FrontToIdle");
            isFrontMoving = false;
        }
    }
}
