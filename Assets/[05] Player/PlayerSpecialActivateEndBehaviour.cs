using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecialActivateEndBehaviour : StateMachineBehaviour
{
    FunnelManager funnelManager;
    private void Awake()
    {
        funnelManager = GameObject.Find("FunnelManager").GetComponent<FunnelManager>();
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        funnelManager.ActivateFunnels();
    }
}
