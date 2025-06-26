using System.Collections;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Crosshair Parts")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("Crosshair Container (for rotation)")]
    public RectTransform crosshairParent;

    private Vector2 topDefault, bottomDefault, leftDefault, rightDefault;
    private Vector2 topTarget, bottomTarget, leftTarget, rightTarget;

    private Vector3 defaultScale;
    private Quaternion defaultRotation;

    private float animationDuration = 0.2f;
    private float animationElapsed = 0f;
    private bool isAnimating = false;

    private Coroutine scaleRoutine;
    private Coroutine rotationRoutine;

    private void Start()
    {
        // Save default positions
        topDefault = top.anchoredPosition;
        bottomDefault = bottom.anchoredPosition;
        leftDefault = left.anchoredPosition;
        rightDefault = right.anchoredPosition;

        topTarget = topDefault;
        bottomTarget = bottomDefault;
        leftTarget = leftDefault;
        rightTarget = rightDefault;

        // Save default scale and rotation
        defaultScale = crosshairParent.localScale;
        defaultRotation = crosshairParent.localRotation;
    }

    private void Update()
    {
        if (isAnimating)
        {
            animationElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(animationElapsed / animationDuration);

            top.anchoredPosition = Vector2.Lerp(top.anchoredPosition, topTarget, t);
            bottom.anchoredPosition = Vector2.Lerp(bottom.anchoredPosition, bottomTarget, t);
            left.anchoredPosition = Vector2.Lerp(left.anchoredPosition, leftTarget, t);
            right.anchoredPosition = Vector2.Lerp(right.anchoredPosition, rightTarget, t);

            if (t >= 1f) isAnimating = false;
        }
    }

    public void Expand(float duration, float spreadAmount)
    {
        animationDuration = duration;
        animationElapsed = 0f;
        isAnimating = true;

        topTarget = topDefault + Vector2.up * spreadAmount;
        bottomTarget = bottomDefault + Vector2.down * spreadAmount;
        leftTarget = leftDefault + Vector2.left * spreadAmount;
        rightTarget = rightDefault + Vector2.right * spreadAmount;
    }

    public void Collapse(float duration)
    {
        animationDuration = duration;
        animationElapsed = 0f;
        isAnimating = true;

        topTarget = topDefault;
        bottomTarget = bottomDefault;
        leftTarget = leftDefault;
        rightTarget = rightDefault;
    }

    public void Pulse(float expandAmount, float expandDuration, float collapseDuration)
    {
        StopAllCoroutines();
        StartCoroutine(PulseRoutine(expandAmount, expandDuration, collapseDuration));
    }

    private IEnumerator PulseRoutine(float amount, float expandTime, float collapseTime)
    {
        Expand(expandTime, amount);
        yield return new WaitForSeconds(expandTime);
        Collapse(collapseTime);
    }

    public void ExpandScale(float duration, float scaleAmount)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine(defaultScale, Vector3.one * scaleAmount, duration));
    }

    public void CollapseScale(float duration)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine(crosshairParent.localScale, defaultScale, duration));
    }

    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            crosshairParent.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        crosshairParent.localScale = to;
    }

    public void RotateCrosshair(float duration, float angle)
    {
        if (rotationRoutine != null) StopCoroutine(rotationRoutine);
        rotationRoutine = StartCoroutine(RotateRoutine(crosshairParent.localRotation, Quaternion.Euler(0, 0, angle), duration));
    }

    public void ResetRotation(float duration)
    {
        if (rotationRoutine != null) StopCoroutine(rotationRoutine);
        rotationRoutine = StartCoroutine(RotateRoutine(crosshairParent.localRotation, defaultRotation, duration));
    }

    private IEnumerator RotateRoutine(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            crosshairParent.localRotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }
        crosshairParent.localRotation = to;
    }
}
