using UnityEngine;
using System.Collections;

public class Camera3D : MonoBehaviour
{
    [Header("Camera Movement")]
    public Transform defaultPosition;
    public Transform specialPosition;
    public float transitionDuration = 1.5f;
    public float holdDuration = 2f;

    private bool isMoving = false;
    public CameraRecoil cameraRecoil;

    [Header("Camera FOV")]
    public Camera targetCamera;
    public float defaultFOV = 60f;
    public float specialFOV = 40f;
    private float fovTransitionDuration;

    void Start()
    {
        fovTransitionDuration = transitionDuration;
    }

    void Update()
    {
    }

    public void PlayCinematic()
    {
        if (!isMoving)
            StartCoroutine(MoveCameraSequence());
    }

    private IEnumerator MoveCameraSequence()
    {
        isMoving = true;

        if (cameraRecoil != null)
            cameraRecoil.IsCinematic = true;

        StartCoroutine(LerpFOV(defaultFOV, specialFOV, fovTransitionDuration));
        yield return StartCoroutine(MoveToPosition(specialPosition));

        yield return new WaitForSeconds(holdDuration);

        StartCoroutine(LerpFOV(specialFOV, defaultFOV, fovTransitionDuration));
        yield return StartCoroutine(MoveToPosition(defaultPosition));

        if (cameraRecoil != null)
            cameraRecoil.IsCinematic = false;

        isMoving = false;
    }


    private IEnumerator MoveToPosition(Transform target)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float t = 0f;

        // Safety check to prevent transitionDuration from being zero
        float duration = Mathf.Max(transitionDuration, 0.01f);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        // Ensure final frame snaps exactly to target
        transform.position = endPos;
        transform.rotation = endRot;
    }

    private IEnumerator LerpFOV(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            targetCamera.fieldOfView = Mathf.Lerp(from, to, t);
            yield return null;
        }

        targetCamera.fieldOfView = to;
    }
}
