using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLightFireVFXBehaviour : StateMachineBehaviour
{
    public Transform gun;
    public HUDManager hudManager;

    private void Awake()
    {
        gun = GameObject.Find("Gun").transform;
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();

        enemy.cameraShake?.StartCoroutine(enemy.cameraShake.Shake(0.2f, 0.02f));
        enemy.cameraRecoil.FireRecoil(CameraRecoil.RecoilStrength.Medium);
        hudManager.TriggerVignetteFlash(Color.red, 0.3f, 0.3f);
        hudManager.TriggerFlash();
        hudManager.PulseHUD(1.05f, 0.1f, 0.3f);

        AudioManager.Instance.Play("LIGHT_RANGED 01");

        enemy.Player.TakeDamage(enemy.LightAttackDamage, AttackType.Light);

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
