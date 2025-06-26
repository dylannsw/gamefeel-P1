using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public Transform hudTransform;
    public Material uiFlashMaterial;

    [Header("Flash Settings")]
    public Color normalColor = Color.white;
    public Color flashColor = Color.red;
    public float flashIntensity = 5f;
    public float normalIntensity = 1f;
    public float flashDuration = 0.1f;
    public float fadeBackDuration = 0.5f;

    [Header("Movement Animation Settings")]
    public float moveScale = 0.1f;
    public float leftRightOffset = 10f;
    public float upDownOffset = 10f;
    public float smoothing = 5f;

    private Coroutine flashRoutine;
    private Vector3 defaultScale;
    private Vector3 defaultPosition;

    void Start()
    {
        if (uiFlashMaterial != null)
        {
            ApplyColor(normalColor * normalIntensity, normalIntensity);
        }

        if (hudTransform != null)
        {
            defaultScale = hudTransform.localScale;
            defaultPosition = hudTransform.localPosition;
        }
    }

    void Update()
    {
        if (player == null || hudTransform == null) return;

        Vector2 moveInput = player.Controls.Movement.ReadValue<Vector2>();
        bool isJumping = player.Controls.Jump.IsPressed();
        bool isDescending = player.Controls.Descend.IsPressed();

        // Z-axis scale adjustment for forward/backward movement
        float scaleFactor = 1f;
        if (moveInput.y > 0.1f) scaleFactor += moveScale;
        else if (moveInput.y < -0.1f) scaleFactor -= moveScale;

        // X and Y position offsets
        float xOffset = moveInput.x * leftRightOffset;
        float yOffset = 0f;
        if (isJumping) yOffset += upDownOffset;
        if (isDescending) yOffset -= upDownOffset;

        Vector3 targetScale = defaultScale * scaleFactor;
        Vector3 targetPosition = defaultPosition + new Vector3(xOffset, yOffset, 0f);

        hudTransform.localScale = Vector3.Lerp(hudTransform.localScale, targetScale, Time.deltaTime * smoothing);
        hudTransform.localPosition = Vector3.Lerp(hudTransform.localPosition, targetPosition, Time.deltaTime * smoothing);
    }

    public void TriggerFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashSequence());
    }

    private IEnumerator FlashSequence()
    {
        if (uiFlashMaterial == null) yield break;

        ApplyColor(flashColor * flashIntensity, flashIntensity);
        yield return new WaitForSeconds(flashDuration);

        float t = 0f;
        Color startColor = uiFlashMaterial.GetColor("_Color");
        float startIntensity = uiFlashMaterial.GetFloat("_Intensity");
        Color targetColor = normalColor * normalIntensity;

        while (t < fadeBackDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / fadeBackDuration);

            Color lerpedColor = Color.Lerp(startColor, targetColor, progress);
            float lerpedIntensity = Mathf.Lerp(startIntensity, normalIntensity, progress);

            ApplyColor(lerpedColor, lerpedIntensity);
            yield return null;
        }

        ApplyColor(targetColor, normalIntensity);
    }

    private void ApplyColor(Color combinedColor, float intensity)
    {
        if (uiFlashMaterial != null)
        {
            uiFlashMaterial.SetColor("_Color", combinedColor);
            uiFlashMaterial.SetFloat("_Intensity", intensity);
        }
    }
}
