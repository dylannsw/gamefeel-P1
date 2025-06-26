using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

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

    [Header("Post Processing")]
    public Volume postProcessingVolume;
    private Vignette vignette;
    private Coroutine flashRoutine;
    private Vector3 defaultScale;
    private Vector3 defaultPosition;
    private Coroutine pulseRoutine;
    private Coroutine warningRoutine;
    private Coroutine vignetteLerpRoutine;

    [Header("Special Warning Image")]
    public Image lightWarningImage;
    public Image mediumWarningImage;
    public Image heavyWarningImage;
    public Image ongoingWarningImage;
    private Dictionary<AttackType, Image> warningImages = new();
    private Dictionary<AttackType, Coroutine> activeWarningRoutines = new();
    public float warningHoldTime = 1.2f;
    public float warningFadeOutTime = 0.5f;

    void Awake()
    {
        warningImages[AttackType.Light] = lightWarningImage;
        warningImages[AttackType.Medium] = mediumWarningImage;
        warningImages[AttackType.Heavy] = heavyWarningImage;
        warningImages[AttackType.None] = ongoingWarningImage;
    }

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

        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }
    }

    void Update()
    {
        if (player == null || hudTransform == null) return;

        if (player.CurrentHP <= 0) LerpVignette(Color.red, 0.6f, 2f);

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

    public void PulseHUD(float scaleAmount, float holdDuration, float returnDuration)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(HUDPulseRoutine(scaleAmount, holdDuration, returnDuration));
    }

    private IEnumerator HUDPulseRoutine(float scaleAmount, float holdDuration, float returnDuration)
    {
        if (hudTransform == null) yield break;

        Vector3 targetScale = defaultScale * scaleAmount;

        //Scale up quickly
        float t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / 0.1f);
            hudTransform.localScale = Vector3.Lerp(defaultScale, targetScale, progress);
            yield return null;
        }

        //Hold scaled
        hudTransform.localScale = targetScale;
        yield return new WaitForSeconds(holdDuration);

        //Return to normal
        t = 0f;
        Vector3 startScale = hudTransform.localScale;
        while (t < returnDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / returnDuration);
            hudTransform.localScale = Vector3.Lerp(startScale, defaultScale, progress);
            yield return null;
        }

        hudTransform.localScale = defaultScale;
    }

    public void TriggerVignetteFlash(Color color, float maxIntensity, float totalDuration)
    {
        if (vignette == null) return;
        StopAllCoroutines(); // Optional: Stops old flashes
        StartCoroutine(VignetteFlashRoutine(color, maxIntensity, totalDuration));
    }

    private IEnumerator VignetteFlashRoutine(Color color, float maxIntensity, float totalDuration)
    {
        float fadeDuration = totalDuration / 2f;
        float fadeOutDuration = totalDuration;

        vignette.color.Override(color);
        vignette.intensity.Override(0f);

        // Fade In
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            vignette.intensity.Override(Mathf.Lerp(0f, maxIntensity, t / fadeDuration));
            yield return null;
        }

        // Fade Out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            vignette.intensity.Override(Mathf.Lerp(maxIntensity, 0f, t / fadeOutDuration));
            yield return null;
        }

        vignette.intensity.Override(0f); // Ensure it's cleanly reset
    }

    public void ShowSpecialBarWarning(AttackType type)
    {
        if (!warningImages.ContainsKey(type) || warningImages[type] == null) return;

        Image img = warningImages[type];

        //Stop any existing coroutine for that type
        if (activeWarningRoutines.ContainsKey(type) && activeWarningRoutines[type] != null)
            StopCoroutine(activeWarningRoutines[type]);

        //Start new fade coroutine
        activeWarningRoutines[type] = StartCoroutine(WarningFadeRoutine(img, type));
    }

    private IEnumerator WarningFadeRoutine(Image img, AttackType type)
    {
        Color visible = img.color;
        visible.a = 0.5f;
        img.color = visible;

        yield return new WaitForSeconds(warningHoldTime);

        float t = 0f;
        Color startColor = img.color;
        Color endColor = startColor;
        endColor.a = 0f;

        while (t < warningFadeOutTime)
        {
            t += Time.deltaTime;
            img.color = Color.Lerp(startColor, endColor, t / warningFadeOutTime);
            yield return null;
        }

        img.color = endColor;
        activeWarningRoutines[type] = null;
    }

    public void LerpVignette(Color targetColor, float targetIntensity, float duration)
    {
        if (vignette == null) return;

        if (vignetteLerpRoutine != null)
            StopCoroutine(vignetteLerpRoutine);

        vignetteLerpRoutine = StartCoroutine(VignetteLerpRoutine(targetColor, targetIntensity, duration));
    }

    private IEnumerator VignetteLerpRoutine(Color targetColor, float targetIntensity, float duration)
    {
        Color startColor = vignette.color.value;
        float startIntensity = vignette.intensity.value;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);

            vignette.color.Override(Color.Lerp(startColor, targetColor, progress));
            vignette.intensity.Override(Mathf.Lerp(startIntensity, targetIntensity, progress));
            yield return null;
        }

        vignette.color.Override(targetColor);
        vignette.intensity.Override(targetIntensity);
    }
}
