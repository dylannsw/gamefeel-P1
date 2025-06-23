using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [System.Serializable]
    public struct RecoilSettings
    {
        public float rotationRecoil;
        public float positionRecoil;
        public float recoilSpeed;
        public float returnSpeed;
    }

    public enum RecoilStrength { Light, Medium, Heavy }

    [Header("Recoil Presets")]
    public RecoilSettings lightRecoil;
    public RecoilSettings mediumRecoil;
    public RecoilSettings heavyRecoil;

    private float currentRotRecoil = 0f;
    private float targetRotRecoil = 0f;

    private Vector3 currentPosRecoil = Vector3.zero;
    private Vector3 targetPosRecoil = Vector3.zero;

    private float recoilSpeed = 20f;
    private float returnSpeed = 10f;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    public bool IsCinematic = false;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    void Update()
    {
        if (IsCinematic) return;

        // Smoothly apply recoil values
        currentRotRecoil = Mathf.Lerp(currentRotRecoil, targetRotRecoil, Time.deltaTime * recoilSpeed);
        currentPosRecoil = Vector3.Lerp(currentPosRecoil, targetPosRecoil, Time.deltaTime * recoilSpeed);

        // Apply to transform
        transform.localRotation = Quaternion.Euler(currentRotRecoil, 0f, 0f);
        transform.localPosition = originalLocalPosition + currentPosRecoil;

        // Smoothly return to rest
        targetRotRecoil = Mathf.Lerp(targetRotRecoil, 0f, Time.deltaTime * returnSpeed);
        targetPosRecoil = Vector3.Lerp(targetPosRecoil, Vector3.zero, Time.deltaTime * returnSpeed);
    }

    public void FireRecoil(RecoilStrength strength)
    {
        RecoilSettings recoil = strength switch
        {
            RecoilStrength.Light => lightRecoil,
            RecoilStrength.Medium => mediumRecoil,
            RecoilStrength.Heavy => heavyRecoil,
            _ => lightRecoil
        };

        // Apply recoil force
        targetRotRecoil += recoil.rotationRecoil;
        targetPosRecoil += Vector3.back * recoil.positionRecoil;

        // Set active speeds
        recoilSpeed = recoil.recoilSpeed;
        returnSpeed = recoil.returnSpeed;
    }
}



