using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavyChargeVFXBehaviour : StateMachineBehaviour
{
    private ParticleSystem chargeVFX;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();

        //weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(7f, 0.0075f));
        AudioManager.Instance.Play("HEAVYENEMY");

        if (enemy != null && enemy.HeavyBeamVFXCharge != null && enemy.BeamSpawnPoint != null)
        {
            chargeVFX = Object.Instantiate(enemy.HeavyBeamVFXCharge, enemy.BeamSpawnPoint.position, enemy.BeamSpawnPoint.rotation);
            chargeVFX.transform.parent = enemy.BeamSpawnPoint;
            chargeVFX.Play();

            //weapon.HeavyBeanVFXHold = chargeVFX;
            Object.Destroy(chargeVFX.gameObject, 10f);
        }
    }
}
