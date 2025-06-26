using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SpecialBarManager : MonoBehaviour
{
    [Header("Special Bar")]
    public Image fillImage; // Reference to Special_Fill
    [Range(0f, 1f)] public float currentFill = 0f;

    [Header("Charge Settings")]
    public float maxCharge = 100f;
    public float currentCharge = 0f;

    [Header("Attack Costs")]
    public float lightCost = 33f;
    public float mediumCost = 66f;
    public float heavyCost = 100f;

    [Header("Recharge Parameters")]
    public float lightRechargeAmount = 5f;
    public float MediumRechargeAmount = 15f;
    public float HeavyRechargeAmount = 30f;

    private void Start()
    {
        EmptyCharge();

    }

    public void AddCharge(float amount)
    {
        currentCharge = Mathf.Clamp(currentCharge + amount, 0f, maxCharge);
        UpdateSpecialHUD();
    }

    public bool SpendCharge(float amount)
    {
        if (currentCharge >= amount)
        {
            currentCharge -= amount;
            UpdateSpecialHUD();
            return true;
        }

        return false;
    }

    public void EmptyCharge()
    {
        currentCharge = 0f;
        UpdateSpecialHUD();
    }

    public void UpdateSpecialHUD()
    {
        fillImage.fillAmount = (float)currentCharge / maxCharge;
    }

    //Optional: helper methods
    public bool CanUseLight() => currentCharge >= lightCost;
    public bool CanUseMedium() => currentCharge >= mediumCost;
    public bool CanUseHeavy() => currentCharge >= heavyCost;
    public float GetCurrentChargePercent() => currentCharge / maxCharge;
}
