using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMedFireVFXBehaviour : StateMachineBehaviour
{
    public HUDManager hudManager;

    private void Awake()
    {
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();

        enemy.cameraShake?.StartCoroutine(enemy.cameraShake.Shake(0.3f, 0.04f));
        enemy.cameraRecoil.FireRecoil(CameraRecoil.RecoilStrength.Medium);
        hudManager.TriggerVignetteFlash(Color.red, 0.5f, 0.3f);
        hudManager.TriggerFlash();
        hudManager.PulseHUD(1.08f, 0.1f, 0.3f);

        AudioManager.Instance.Play("LIGHT_RANGED SPECIAL 02");

        enemy.Player.TakeDamage(enemy.MediumAttackDamage, AttackType.Medium);

        if (enemy != null && enemy.MedBeamVFXFire != null && enemy.BeamSpawnPoint != null)
        {
            Vector3 targetPos = enemy.Player.transform.position + Vector3.down * 0.35f;

            Vector3 fireDirection = (targetPos - enemy.BeamSpawnPoint.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            // Instantiate the stretched beam cone VFX
            ParticleSystem beam = Object.Instantiate(enemy.MedBeamVFXFire, enemy.BeamSpawnPoint.position, rotation);
            beam.Play();
            Object.Destroy(beam.gameObject, 2f);
        }
    }
}
