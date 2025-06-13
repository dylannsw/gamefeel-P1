using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyChargeVFXBehaviour : StateMachineBehaviour
{
    private ParticleSystem chargeVFX;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        
        //Reduce Mouse Sensitivity
        weapon.Player.Sensitivity = weapon.attackSensitivity;

        weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(4f, 0.0075f));

        if (weapon != null && weapon.HeavyBeamVFXCharge != null && weapon.BeamSpawnPoint != null)
        {
            chargeVFX = Object.Instantiate(weapon.HeavyBeamVFXCharge, weapon.BeamSpawnPoint.position, weapon.BeamSpawnPoint.rotation);
            chargeVFX.transform.parent = weapon.BeamSpawnPoint;
            chargeVFX.Play();

            weapon.HeavyBeanVFXHold = chargeVFX;
        }
    }
}
