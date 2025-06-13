using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyFireVFXBehaviour : StateMachineBehaviour
{
    public float BeamLifetime = 5f;
    public Transform gun;

    private void Awake()
    {
        gun = GameObject.Find("Gun").transform;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        if (weapon != null && weapon.HeavyBeamVFXFire != null && weapon.BeamSpawnPoint != null)
        {
            //Get direction from camera center (crosshair)
            Vector3 fireDirection = weapon.Player.PlayerCamera.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the beam
            ParticleSystem beam = Object.Instantiate(weapon.HeavyBeamVFXFire, weapon.BeamSpawnPoint.position, rotation, gun);
            beam.Play();

            if (weapon != null)
            {
                // Stop/reset FOV after heavy fire animation ends
                if (weapon.fovCoroutine != null)
                    weapon.StopCoroutine(weapon.fovCoroutine);

                weapon.fovCoroutine = weapon.StartCoroutine(weapon.ChangeFOV(weapon.defaultFOV));
            }
            Object.Destroy(beam.gameObject, BeamLifetime);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();
        if (weapon != null && weapon.Player != null)
        {
            // Stop existing coroutine if it's running
            if (weapon.Player.SensitivityCoroutine != null)
                weapon.StopCoroutine(weapon.Player.SensitivityCoroutine);

            // Smoothly restore sensitivity over 0.5 seconds
            weapon.Player.SensitivityCoroutine = weapon.StartCoroutine(
                weapon.Player.SmoothRestoreSensitivity(weapon.Player.DefaultSensitivity, 1.5f)
            );
        }
    }
}

