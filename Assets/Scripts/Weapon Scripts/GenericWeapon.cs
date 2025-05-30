using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GenericWeapon : Weapon
{
    [Header("Player Reference")]
    public PlayerController Player;

    [Header("Attack Delays")]
    public float InitialLightAttackDelay = 0f;
    public float InitialMediumAttackDelay = 0.5f;
    public float InitialHeavyAttackDelay = 1f;

    //Add custom variables for weapon here.



    //

    private void Awake()
    {
        //Make a reference to our PlayerController
        Player = FindObjectOfType<PlayerController>();

        //Set up our controls to match those referenced
        InitializeControls(Player);
    }

    private void Update()
    {
        
    }

    private void UpdateHUD()
    {
        //Custom implementation here.
    }

    public override void OnWeaponEquip()
    {
        //Custom implementation here.
    }

    public override void OnWeaponUnEquip()
    {
        //Custom implementation here.
    }

    public override void PerformLightAttack()
    {
        if (!IsAttacking /* && Custom Variable */)
        {
            CurrentAttack = AttackType.Light;

            //Invoke attack animation.
            LightAttackEvent?.Invoke();

            //Call attack to begin after a set delay.
            StartCoroutine(PerformAttack(InitialLightAttackDelay));
        }
    }

    public override void PerformMediumAttack()
    {
        if (!IsAttacking /* && Custom Variable */)
        {
            CurrentAttack = AttackType.Medium;

            //Invoke attack animation.
            MediumAttackEvent?.Invoke();

            //Call attack to begin after a set delay.
            StartCoroutine(PerformAttack(InitialMediumAttackDelay));
        }
    }

    public override void PerformHeavyAttack()
    {
        if (!IsAttacking /* && Custom Variable */)
        {
            CurrentAttack = AttackType.Heavy;

            //Invoke attack animation.
            HeavyAttackEvent?.Invoke();

            //Call attack to begin after a set delay.
            StartCoroutine(PerformAttack(InitialHeavyAttackDelay));
        }
    }

    public IEnumerator PerformAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch(CurrentAttack)
        {
            case AttackType.Light:

                //Insert custom behavior here.

                /*
                TIP: Call 'ReceiveDamage()' from enemy script to apply damage to the
                enemy. That might be put here or on an object that is "hitting" the enemy
                instead. Either way, it will need to be called for damage to be applied.
                */

                break;

            case AttackType.Medium:

                //Insert custom behavior here.

                /*
                TIP: Call 'ReceiveDamage()' from enemy script to apply damage to the
                enemy. That might be put here or on an object that is "hitting" the enemy
                instead. Either way, it will need to be called for damage to be applied.
                */

                break;

            case AttackType.Heavy:

                //Insert custom behavior here.

                /*
                TIP: Call 'ReceiveDamage()' from enemy script to apply damage to the
                enemy. That might be put here or on an object that is "hitting" the enemy
                instead. Either way, it will need to be called for damage to be applied.
                */

                break;
        }
    }

    //Insert custom functions here.

    /*
     Refer to 'ExampleMelee.cs' and 'ExampleHitscanRanged.cs' scripts for any specific 
     custom implementation such as Updating a custom HUD or implementing time dilation.
    */
}
    