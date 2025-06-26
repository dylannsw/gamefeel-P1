using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedFireVFXBehaviour : StateMachineBehaviour
{
    public float BeamLifetime = 10f;
    public Transform gun;
    public SpecialBarManager specialBarHUD;
    public CrosshairController crosshairController;

    private void Awake()
    {
        gun = GameObject.Find("Gun").transform;
        specialBarHUD = GameObject.Find("SpecialBarManager").GetComponent<SpecialBarManager>();
        crosshairController = GameObject.Find("CrosshairManager").GetComponent<CrosshairController>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        ExampleHitscanRanged weapon = animator.GetComponentInParent<ExampleHitscanRanged>();

        if (weapon != null) weapon.CurrentAmmo -= weapon.MediumAmmoCost; //Update Ammo during Fire State only
        if (weapon != null) weapon.FireRaycast(); //Fire Raycast (Only for Medium and Heavy)
        weapon.UpdateHUD();

        //crosshairController.Expand(0.75f, 20f);

        if (weapon != null && weapon.MedBeamVFXFire != null && weapon.MedBeamSpawnPoint != null)
        {
            //Get direction from camera center (crosshair)
            Vector3 fireDirection = weapon.Player.PlayerCamera.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            //Instantiate the beam
            ParticleSystem beam = Object.Instantiate(weapon.MedBeamVFXFire, weapon.MedBeamSpawnPoint.position, rotation, gun);
            beam.Play();

            if (weapon != null && weapon.MedBeamVFX != null)
            {
                //Send the default spawn event to the VFX Graph
                weapon.MedBeamVFX.SendEvent("OnPlay");
            }

            if (weapon != null)
            {
                CameraRecoil recoil = weapon.Player.PlayerCamera.transform.parent.GetComponent<CameraRecoil>();
                if (recoil != null)
                {
                    recoil.FireRecoil(CameraRecoil.RecoilStrength.Medium);
                }
            }

            // if (weapon != null)
            // {
            //     //Stop/reset FOV after heavy fire animation ends
            //     if (weapon.fovCoroutine != null)
            //         weapon.StopCoroutine(weapon.fovCoroutine);

            //     weapon.fovCoroutine = weapon.StartCoroutine(weapon.ChangeFOV(weapon.defaultFOV));
            // }
            Object.Destroy(beam.gameObject, BeamLifetime);

            specialBarHUD.AddCharge(specialBarHUD.MediumRechargeAmount);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        crosshairController.Collapse(1f);
    }
}

