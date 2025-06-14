using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class LightVFXFireBehaviour : StateMachineBehaviour
{
    public Transform gun;

    private void Awake()
    {
        gun = GameObject.Find("Gun").transform;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();

        //weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(0.5f, 0.005f));

        if (weapon != null && weapon.LightBeamVFXFire != null && weapon.LightBeamSpawnPoint != null)
        {
            Vector3 fireDirection = weapon.Player.PlayerCamera.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the stretched beam cone VFX
            ParticleSystem beam = Object.Instantiate(weapon.LightBeamVFXFire, weapon.LightBeamSpawnPoint.position, rotation, gun);
            beam.Play();
            Object.Destroy(beam.gameObject, 0.6f);

            if (weapon != null && weapon.LightMuzzleVFX != null)
            {
                //Send the default spawn event to the VFX Graph
                weapon.LightMuzzleVFX.SendEvent("OnPlay");
            }
        }
    }

}


