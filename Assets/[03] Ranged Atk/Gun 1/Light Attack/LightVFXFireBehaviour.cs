using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightVFXFireBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        if (weapon != null && weapon.LightBeamVFXFire != null && weapon.LightBeamSpawnPoint != null)
        {
            //Get direction from camera center (crosshair)
            Vector3 fireDirection = weapon.Player.PlayerCamera.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the beam
            ParticleSystem beam = Object.Instantiate(weapon.LightBeamVFXFire, weapon.LightBeamSpawnPoint.position, rotation);
            beam.Play();

            Object.Destroy(beam.gameObject, 0.6f);
        }
    }
}


