using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedChargeVFXBehaviour : StateMachineBehaviour
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

        //weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(7f, 0.0075f));
        crosshairController.Expand(0.75f, 20f);

        if (weapon != null && weapon.MedBeamVFXCharge != null && weapon.MedChargeSpawnPoint != null)
        {
            chargeVFX = Object.Instantiate(weapon.MedBeamVFXCharge, weapon.MedChargeSpawnPoint.position, weapon.MedChargeSpawnPoint.rotation);
            chargeVFX.transform.parent = weapon.MedChargeSpawnPoint;
            chargeVFX.Play();

            //weapon.HeavyBeanVFXHold = chargeVFX;
            Object.Destroy(chargeVFX.gameObject, 5);
        }
    }
}
