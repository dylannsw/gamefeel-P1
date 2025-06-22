using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavyFireVFXBehaviour : StateMachineBehaviour
{
    public Transform gundam;

    private void Awake()
    {
        gundam = GameObject.Find("TrackingRef").transform;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();

        //weapon.cameraShake?.StartCoroutine(weapon.cameraShake.Shake(0.5f, 0.005f));

        if (enemy != null && enemy.HeavyBeamVFXFire != null && enemy.BeamSpawnPoint != null)
        {
            Vector3 targetPos = enemy.Player.transform.position + Vector3.down * 0.25f;

            Vector3 fireDirection = (targetPos - enemy.BeamSpawnPoint.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the stretched beam cone VFX
            ParticleSystem beam = Object.Instantiate(enemy.HeavyBeamVFXFire, enemy.BeamSpawnPoint.position + Vector3.down * 0.7f, rotation, gundam);
            beam.Play();
            Object.Destroy(beam.gameObject, 6f);

            if (enemy != null && enemy.HeavyBeamVFX != null)
            {
                //Send the default spawn event to the VFX Graph
                enemy.HeavyBeamVFX.SendEvent("OnPlay");
            }
        }
    }
}
