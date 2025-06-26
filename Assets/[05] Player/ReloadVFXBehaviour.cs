using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadVFXBehaviour : StateMachineBehaviour
{
    public Transform barrelSpawnPos;
    public PlayerController player;

    private void Awake()
    {
        barrelSpawnPos = GameObject.Find("ReloadVFXSpawn").transform;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ParticleSystem vfx = Object.Instantiate(player.reloadVFX, barrelSpawnPos.position, barrelSpawnPos.rotation, barrelSpawnPos);
        vfx.Play();
        Object.Destroy(vfx.gameObject, 6f);
    }
}
