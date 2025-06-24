using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecialToIdleEndBehaviour : StateMachineBehaviour
{
    FunnelManager funnelManager;
    private void Awake()
    {
        funnelManager = GameObject.Find("FunnelManager").GetComponent<FunnelManager>();
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (funnelManager.funnelMode == 1) funnelManager.ActivateFunnels(FunnelManager.FunnelAttackMode.Heavy);
        else if (funnelManager.funnelMode == 2) funnelManager.ActivateFunnels(FunnelManager.FunnelAttackMode.Medium);
        else if (funnelManager.funnelMode == 3) funnelManager.ActivateFunnels(FunnelManager.FunnelAttackMode.Light);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        funnelManager.funnelMode = 0; //Reset funnel
    }
}
