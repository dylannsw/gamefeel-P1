using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float Gravity = 0f; //Changed to 0 because zero gravity evironment
    public float JumpStrength = 1.5f;
    //public bool IsGrounded; //Not required anymore no ground check
    public float acceleration = 20f;
    public float maxSpeed = 7f;
    public Vector3 Velocity;

    [Header("Camera")]
    public GameObject FPSRig;
    public Camera PlayerCamera;
    public float DefaultSensitivity = 25f;
    public float Sensitivity;
    public Coroutine SensitivityCoroutine;

    [Header("PlayerHUD")]
    public GameObject HealthBarHUD;
    public Image HealthBarFill;
    public GameObject PauseMenu;
    public TextMeshProUGUI HealthText;
    private bool[] segmentBroken;
    public HUDManager hUDManager;
    public GameObject AmmoBarHUD;
    public GameObject SpecialBarHUD;

    [Header("Player Health Bar HUD")]
    public Animator[] SegmentAnimators;
    public int TotalSegments = 10;
    private int _lastSegmentCount;

    [Header("Combat")]
    public float MaxHP = 100f;
    public float CurrentHP = 100f;
    public UnityEvent TakeLightDamage;
    public UnityEvent TakeMediumDamage;
    public UnityEvent TakeHeavyDamage;
    public UnityEvent OnDeath;
    public Weapon CurrentWeapon;
    public List<GameObject> WeaponList = new List<GameObject>();
    public ParticleSystem reloadVFX;
    public ExampleHitscanRanged weapon;

    [Header("Special Attack")]
    public Camera3D camera3D;
    public UnityEvent OnHeavySpecialActivated;
    public UnityEvent OnMediumSpecialActivated;
    public UnityEvent OnLightSpecialActivated;
    public FunnelManager funnelManager;
    public SpecialBarManager specialBarHUD;

    [Header("Special Attack Animation Intros")]
    public UnityEvent OnLightSpecialIntro;
    public UnityEvent OnMediumSpecialIntro;
    public UnityEvent OnHeavySpecialIntro;

    [Header("Death UI")]
    public GameOverHandler gameOverHandler;
    private bool isDead = false;

    [HideInInspector]
    public PlayerControls.BasicActions Controls;
    private CharacterController CC;
    private PlayerControls PC;
    float xRotation;
    float _Gravity;
    bool GamePaused;
    public float PitchVariation = 0.2f;



    private void Awake()
    {
        _Gravity = Gravity;
        CC = GetComponent<CharacterController>();
        Weapon[] foundWeapons = GetComponentsInChildren<Weapon>(true);
        foreach (Weapon weapon in foundWeapons)
        {
            WeaponList.Add(weapon.gameObject);
        }
        EquipWeapon(WeaponList[0]);
        Cursor.lockState = CursorLockMode.Locked;
        GamePaused = false;
        Time.timeScale = 1;
    }

    private void Start()
    {
        //Health Bar Damage Animations
        _lastSegmentCount = TotalSegments;
        segmentBroken = new bool[TotalSegments];
        for (int i = 0; i < TotalSegments; i++) segmentBroken[i] = false;

        PauseMenu.SetActive(false);

        UpdateHUD();
    }

    private void OnEnable()
    {
        /*
        WHILE YOU ARE FREE TO MESS WITH VALUES FOR THE CHARACTER CONTROLLER, REFRAIN
        FROM DIRECTLY EDITING CONTROLLER-RELEATED IMPLEMENTATION IN THIS SCRIPT OR
        THE 'PlayerControls.cs' SCRIPT. THE IMPLEMENTATION WAS DONE IN A VERY
        PARTICULAR WAY FOR THE PURPOSES OF THIS PROJECT.

        ALL NECESSARY IMPLEMENTATION FOR KEYBOARD+MOUSE AND CONTROLLER(GAMEPAD)*
        SUPPORT IS PRESENT AND SHOULD WORK ALREADY.

        *Playstation controllers experience difficulties when implementing them.
        If this is the case, please consider checking out an xbox controller
        from the DigiPen library.
        */

        PC = new PlayerControls();
        PC.Enable();

        //Assign and reference our methods to our controls from the IM
        Controls = PC.Basic;
        Controls.Jump.performed += ctx => Jump();
        Controls.LightAttack.performed += ctx => CurrentWeapon?.PerformLightAttack();
        Controls.MediumAttack.performed += ctx => CurrentWeapon?.PerformMediumAttack();
        Controls.HeavyAttack.performed += ctx => CurrentWeapon?.PerformHeavyAttack();
        Controls.Reload.performed += ctx => CurrentWeapon?.PerformReload();
        Controls.PauseGame.performed += ctx => TogglePause();

        Debug.Log("Descend binding: " + Controls.Descend.bindings.Count);
    }

    // Update is called once per frame
    void Update()
    {
        //GroundedCheck(); //Removed gorund check, no longer needed
        //GetInventoryInput();
        if (Keyboard.current.hKey.wasPressedThisFrame) Heal(40);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (funnelManager.FunnelAttackState())
            {
                //Player is already in a funnel attack, show "Ongoing Attack" warning
                hUDManager.ShowSpecialBarWarning(AttackType.None);
                AudioManager.Instance.Play("UI_ERROR");
            }
            else if (!specialBarHUD.CanUseLight())
            {
                //Player is not in a funnel attack, but does not have enough charge
                hUDManager.ShowSpecialBarWarning(AttackType.Light);
                AudioManager.Instance.Play("UI_ERROR");
            }
            else
            {
                TriggerLightSpecial();
                specialBarHUD.SpendCharge(specialBarHUD.lightCost);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (funnelManager.FunnelAttackState())
            {
                //Player is already in a funnel attack, show "Ongoing Attack" warning
                hUDManager.ShowSpecialBarWarning(AttackType.None);
                AudioManager.Instance.Play("UI_ERROR");
            }
            else if (!specialBarHUD.CanUseMedium())
            {
                //Player is not in a funnel attack, but does not have enough charge
                hUDManager.ShowSpecialBarWarning(AttackType.Medium);
                AudioManager.Instance.Play("UI_ERROR");
            }
            else if (weapon.CurrentAmmo <= weapon.MediumAmmoCost)
            {
                hUDManager.ShowSpecialBarWarning(AttackType.None);
                AudioManager.Instance.Play("UI_ERROR");
                weapon.PerformReload();
            }
            else
            {
                TriggerMediumSpecial();
                specialBarHUD.SpendCharge(specialBarHUD.mediumCost);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (funnelManager.FunnelAttackState())
            {
                //Player is already in a funnel attack, show "Ongoing Attack" warning
                hUDManager.ShowSpecialBarWarning(AttackType.None);
                AudioManager.Instance.Play("UI_ERROR");
            }
            else if (!specialBarHUD.CanUseHeavy())
            {
                //Player is not in a funnel attack, but does not have enough charge
                hUDManager.ShowSpecialBarWarning(AttackType.Heavy);
                AudioManager.Instance.Play("UI_ERROR");
            }
            else
            {
                TriggerHeavySpecial();
                specialBarHUD.SpendCharge(specialBarHUD.heavyCost);
            }
        }

    }

    private void EquipWeapon(GameObject weapon)
    {
        if (!WeaponList.Contains(weapon))
        {
            Debug.LogError($"{weapon.name} does not exist in weapon inventory");
            return;
        }
        else
        {
            try
            {
                foreach (GameObject obj in WeaponList)
                {
                    obj.gameObject.SetActive(false);
                }
                weapon.gameObject.SetActive(true);

                if (CurrentWeapon != null)
                    CurrentWeapon.WeaponUnequipped();

                CurrentWeapon = weapon.GetComponent<Weapon>();
                CurrentWeapon.WeaponEquipped();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{weapon.name} could not be equipped, error: {ex}");
                return;
            }
        }
    }

    // public int GetPressedNumber()
    // {
    //     //Helper function-- if the keycode's ToString returns a number, return it.
    //     for (int number = 0; number <= 9; number++)
    //     {
    //         if (Input.GetKeyDown(number.ToString()))
    //             return number;
    //     }
    //     return -1;
    // }

    // private void GetInventoryInput()
    // {
    //     //If the player presses a number, attempt to equip a weapon at that inventory slot
    //     if (GetPressedNumber() > -1 && !CurrentWeapon.IsAttacking)
    //         try { EquipWeapon(WeaponList[GetPressedNumber() - 1]); }
    //         catch { Debug.Log("No weapon at index " + (GetPressedNumber() - 1).ToString()); return; }
    // }

    public void TogglePause()
    {
        AudioManager.Instance.Play("UI_TOGGLE");
        GamePaused = !GamePaused;
        PauseMenu.SetActive(GamePaused);
        HealthBarHUD.SetActive(!GamePaused);
        AmmoBarHUD.SetActive(!GamePaused);
        SpecialBarHUD.SetActive(!GamePaused);

        if (GamePaused)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = GamePaused ? 0 : 1;
        return;
    }

    private void CameraLook()
    {
        //Grab the movement values from our Control's mouseLook config
        float mouseX = Controls.Look.ReadValue<Vector2>().x;
        float mouseY = Controls.Look.ReadValue<Vector2>().y;

        //Store our vertical rotation movement into a value to apply to transform
        xRotation -= (mouseY * Time.deltaTime) * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 85f);

        //Apply our transforms to both our camera (vertical) and our player (horizontal)
        FPSRig.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * Sensitivity);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    private void MovePlayer()
    {
        //Grab our WASD input as a vector2 and normalize it with our transform
        Vector2 controlAxis = Controls.Movement.ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(controlAxis.x, 0, controlAxis.y);
        inputDir = (transform.right * controlAxis.x + transform.forward * controlAxis.y).normalized;

        //Add velocity each frame
        // Velocity.y += Gravity * Time.deltaTime;

        //If we're grounded, apply a constant downwards force of baseGravity / 2 for slopes.
        // if (IsGrounded && Velocity.y < 0)
        //     Velocity.y = _Gravity / 2;

        //Apply our movements
        // CC.Move((inputVelocity * MoveSpeed) * Time.deltaTime + //PlayerInput Movement
        //          Velocity * Time.deltaTime); //Additional Velocities (Jump/Gravity)

        //Vertical movement
        Vector3 verticalThrust = Vector3.zero;
        if (Controls.Jump.IsPressed()) verticalThrust += Vector3.up;
        if (Controls.Descend.IsPressed()) verticalThrust += Vector3.down;

        Vector3 thrustInput = (inputDir + verticalThrust).normalized;

        // Apply acceleration only if input is given
        if (thrustInput.magnitude > 0.1f)
        {
            Velocity += thrustInput * acceleration * Time.deltaTime;
            Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed); // Prevent infinite speed
        }
        else
        {
            // Apply damping when not thrusting
            float damping = 8f;
            Velocity = Vector3.Lerp(Velocity, Vector3.zero, damping * Time.deltaTime);
        }

        // Apply movement
        CC.Move(Velocity * Time.deltaTime);
    }

    // private void GroundedCheck()
    // {
    //     IsGrounded = CC.isGrounded;
    // }

    private void Jump()
    {
        //if (IsGrounded) Remove because no longer on the ground, in space
        Velocity.y = JumpStrength;
    }

    public void TakeDamage(int damage, AttackType strength)
    {
        if (isDead) return;

        CurrentHP -= damage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);

        // hUDManager.TriggerFlash(); //Trigger HUD Damage Flash

        UpdateHUD();

        if (CurrentHP <= 0)
        {
            Velocity = Vector3.zero;
            PC.Disable();
            OnDeath?.Invoke();
            HealthBarHUD.SetActive(false);
            AmmoBarHUD.SetActive(false);
            SpecialBarHUD.SetActive(false);
            gameOverHandler.TriggerGameOver(); // fade screen and show UI
            specialBarHUD.EmptyCharge();
            return;
        }

        switch (strength)
        {
            case AttackType.Light:
                TakeLightDamage.Invoke();
                break;

            case AttackType.Medium:
                TakeMediumDamage.Invoke();
                break;

            case AttackType.Heavy:
                TakeHeavyDamage.Invoke();
                break;
        }

        //Space for custom implementation
    }

    private void UpdateHUD()
    {
        HealthBarFill.fillAmount = (float)CurrentHP / MaxHP;
        HealthText.text = $"{CurrentHP}/{MaxHP}";

        float hpPercent = CurrentHP / MaxHP;


        for (int i = 0; i < TotalSegments; i++)
        {
            float threshold = (float)i / TotalSegments;

            // Break segment if HP is below its range
            if (!segmentBroken[i] && hpPercent <= threshold)
            {
                SegmentAnimators[i]?.SetTrigger("Play");
                segmentBroken[i] = true;
            }
            // Reset segment if HP is restored above it
            else if (segmentBroken[i] && hpPercent >= threshold)
            {
                segmentBroken[i] = false;
            }
        }
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);
        UpdateHUD();
    }

    public void PlaySoundWithRandomPitch(AudioClip clip)
    {
        GetComponent<AudioSource>().pitch = 1 + UnityEngine.Random.Range(-PitchVariation, PitchVariation);
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    public IEnumerator SmoothRestoreSensitivity(float targetSensitivity, float duration)
    {
        float startSensitivity = Sensitivity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Sensitivity = Mathf.Lerp(startSensitivity, targetSensitivity, t);
            yield return null;
        }

        Sensitivity = targetSensitivity;
    }

    public void TriggerHeavySpecial()
    {
        StartCoroutine(DelayedHeavySpecial());
    }

    private IEnumerator DelayedHeavySpecial()
    {
        OnHeavySpecialIntro?.Invoke();
        AudioManager.Instance.Play("SPECIALACTIVATION");
        yield return new WaitForSeconds(1.05f);

        if (camera3D != null) camera3D.PlayCinematic();
        AudioManager.Instance.Play("GUNDAM ACTIVATION");
        OnHeavySpecialActivated?.Invoke();
        funnelManager.funnelMode = 1;
        funnelManager.OnFunnelAttackStart();
    }
    public void TriggerMediumSpecial()
    {
        StartCoroutine(DelayedMediumSpecial());
    }

    private IEnumerator DelayedMediumSpecial()
    {
        OnMediumSpecialIntro?.Invoke();
        AudioManager.Instance.Play("SPECIALACTIVATION");
        yield return new WaitForSeconds(1.05f);

        if (camera3D != null) camera3D.PlayCinematic();
        AudioManager.Instance.Play("GUNDAM ACTIVATION");
        OnMediumSpecialActivated?.Invoke();
        funnelManager.funnelMode = 2;
        funnelManager.OnFunnelAttackStart();
    }

    public void TriggerLightSpecial()
    {
        StartCoroutine(DelayedLightSpecial());
    }

    private IEnumerator DelayedLightSpecial()
    {
        OnLightSpecialIntro?.Invoke();
        AudioManager.Instance.Play("SPECIALACTIVATION");

        yield return new WaitForSeconds(1.05f);

        if (camera3D != null) camera3D.PlayCinematic();
        AudioManager.Instance.Play("GUNDAM ACTIVATION");
        OnLightSpecialActivated?.Invoke();
        funnelManager.funnelMode = 3;
        funnelManager.OnFunnelAttackStart();
    }

}
