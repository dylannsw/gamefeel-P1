using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyChargeVFXBehaviour : StateMachineBehaviour
{
    private ParticleSystem chargeVFX;
    public CrosshairController crosshairController;

    private void Awake()
    {
        crosshairController = GameObject.Find("CrosshairManager").GetComponent<CrosshairController>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();

        //Reduce Mouse Sensitivity
        weapon.Player.Sensitivity = weapon.attackSensitivity;

        weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(7f, 0.0075f));

        //Expand Crosshair
        crosshairController.RotateCrosshair(0.5f, 90f);
        crosshairController.Expand(1.5f, 70f);

        if (weapon != null && weapon.HeavyBeamVFXCharge != null && weapon.HeavyChargeSpawnPoint != null)
        {
            chargeVFX = Object.Instantiate(weapon.HeavyBeamVFXCharge, weapon.HeavyChargeSpawnPoint.position, weapon.HeavyChargeSpawnPoint.rotation);
            chargeVFX.transform.parent = weapon.HeavyChargeSpawnPoint;
            chargeVFX.Play();

            weapon.HeavyBeanVFXHold = chargeVFX;
        }
    }
}
