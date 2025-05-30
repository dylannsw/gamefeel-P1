using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum AttackType { Light, Medium, Heavy }
public abstract class Weapon : MonoBehaviour
{
    public PlayerControls.BasicActions Controls;
    public bool IsAttacking = false;

    public UnityEvent EquipEvent;
    public UnityEvent UnEquipEvent;
    public UnityEvent LightAttackEvent;
    public UnityEvent MediumAttackEvent;
    public UnityEvent HeavyAttackEvent;
    public UnityEvent ReloadEvent;

    //If you'd like per-tag hit vfx (different particles per object hit) use the below code
    // public List<TagVFXPair> LightEffects;
    // public List<TagVFXPair> MediumEffects;
    // public List<TagVFXPair> HeavyEffects;

    [HideInInspector]
    public AttackType CurrentAttack;

    public void InitializeControls(PlayerController player)
    {
        IsAttacking = false;
        Controls = player.Controls;
        OnInitialize();
    }

    public void WeaponEquipped()
    {
        Debug.Log($"{this.name} equipped as {this.GetType()}");
        EquipEvent?.Invoke();
        OnWeaponEquip();
    }

    public void WeaponUnequipped()
    {
        Debug.Log($"{this.name} unequipped as {this.GetType()}");
        UnEquipEvent?.Invoke();
        OnWeaponUnEquip();
    }

    public virtual void OnInitialize() { return; }
    public abstract void OnWeaponEquip();
    public abstract void OnWeaponUnEquip();
    public virtual void PerformLightAttack() 
    { 
        LightAttackEvent?.Invoke();
    }
    public virtual void PerformMediumAttack() 
    { 
        MediumAttackEvent?.Invoke();
    }
    public virtual void PerformHeavyAttack() 
    { 
        HeavyAttackEvent?.Invoke();
    }
    public virtual void PerformReload() 
    { 
        ReloadEvent?.Invoke();
    }
}

[System.Serializable]
public class TagVFXPair
{
    [SerializeField] public string tag;
    [SerializeField] public GameObject vfx;
}