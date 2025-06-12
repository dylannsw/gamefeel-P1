using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyFireVFXBehaviour : StateMachineBehaviour
{
    public float BeamLifetime = 2f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        if (weapon != null && weapon.HeavyBeamVFXFire != null && weapon.BeamSpawnPoint != null)
        {
            // Get direction from camera center (crosshair)
            Vector3 fireDirection = weapon.Player.PlayerCamera.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the beam
            ParticleSystem beam = Object.Instantiate(weapon.HeavyBeamVFXFire, weapon.BeamSpawnPoint.position, rotation);
            beam.Play();

            //Object.Destroy(beam.gameObject, BeamLifetime);
        }
    }
}

