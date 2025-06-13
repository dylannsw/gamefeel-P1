using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyVFXBehaviour : StateMachineBehaviour
{
    private ParticleSystem chargeVFX;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        if (weapon != null && weapon.HeavyBeamVFXCharge != null && weapon.BeamSpawnPoint != null)
        {
            chargeVFX = Object.Instantiate(weapon.HeavyBeamVFXCharge, weapon.BeamSpawnPoint.position, weapon.BeamSpawnPoint.rotation);
            chargeVFX.transform.parent = weapon.BeamSpawnPoint;
            chargeVFX.Play();
            
            weapon.HeavyBeanVFXHold = chargeVFX;
        }
    }
}
