using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeavyFireVFXBehaviour : StateMachineBehaviour
{
    public Transform gundam;
    public HUDManager hudManager;

    private void Awake()
    {
        gundam = GameObject.Find("TrackingRef").transform;
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy == null) return;

        enemy.cameraShake?.StartCoroutine(enemy.cameraShake.Shake(3.5f, 0.04f));
        enemy.cameraRecoil.FireRecoil(CameraRecoil.RecoilStrength.Medium);
        hudManager.TriggerVignetteFlash(Color.red, 0.4f, 3.5f);
        hudManager.TriggerFlash();
        hudManager.PulseHUD(1.08f, 0.1f, 0.2f);

        enemy.StartHeavyBeamTick(); // Start tick damage

        if (enemy.HeavyBeamVFXFire != null && enemy.BeamSpawnPoint != null)
        {
            Vector3 targetPos = enemy.Player.transform.position + Vector3.down * 0.25f;
            Vector3 fireDirection = (targetPos - enemy.BeamSpawnPoint.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            ParticleSystem beam = Object.Instantiate(enemy.HeavyBeamVFXFire, enemy.BeamSpawnPoint.position + Vector3.down * 0.7f, rotation, gundam);
            beam.Play();
            Object.Destroy(beam.gameObject, 6f);
        }

        if (enemy.HeavyBeamVFX != null)
            enemy.HeavyBeamVFX.SendEvent("OnPlay");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.StopHeavyBeamTick(); // Stop tick damage
        }
    }
}

