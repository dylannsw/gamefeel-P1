using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    public static GameOverHandler Instance;

    public HUDManager hUDManager;

    [Header("UI References")]
    public Image fadeImage;
    public GameObject deathScreen;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float delayAfterFade = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        // Ensure image starts fully transparent
        if (fadeImage != null)
        {
            Color startColor = fadeImage.color;
            startColor.a = 0f;
            fadeImage.color = startColor;
        }

        // Hide death screen by default
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    public void TriggerGameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(FadeAndShowDeathScreen());
    }

    private IEnumerator FadeAndShowDeathScreen()
    {
        if (fadeImage == null || deathScreen == null)
        {
            Debug.LogWarning("Missing references on GameOverHandler.");
            yield break;
        }

        Color color = fadeImage.color;
        float t = 0f;
        hUDManager.LerpVignette(Color.black, 0.2f, 1f); // Start vignette fade immediately

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / fadeDuration);
            color.a = progress;
            fadeImage.color = color;
            yield return null;
        }
        
        // Delay before showing death screen
        yield return new WaitForSeconds(delayAfterFade);

        deathScreen.SetActive(true);
    }
}
