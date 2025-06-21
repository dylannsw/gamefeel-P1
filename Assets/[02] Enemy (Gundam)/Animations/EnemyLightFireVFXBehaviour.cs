using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLightFireVFXBehaviour : StateMachineBehaviour
{
    public Transform gun;

    private void Awake()
    {
        gun = GameObject.Find("Gun").transform;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();

        //weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(0.5f, 0.005f));

        if (enemy != null && enemy.LightBeamVFXFire != null && enemy.BeamSpawnPoint != null)
        {
            Vector3 targetPos = enemy.Player.transform.position + Vector3.down * 0.25f;

            Vector3 fireDirection = (targetPos - enemy.BeamSpawnPoint.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the stretched beam cone VFX
            ParticleSystem beam = Object.Instantiate(enemy.LightBeamVFXFire, enemy.BeamSpawnPoint.position, rotation);
            beam.Play();
            Object.Destroy(beam.gameObject, 0.6f);

            // if (weapon != null && weapon.LightMuzzleVFX != null)
            // {
            //     weapon.LightMuzzleVFX.SendEvent("OnPlay");
            // }
        }
    }
}
