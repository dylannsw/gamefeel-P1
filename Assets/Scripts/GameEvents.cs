using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartAfterDelay(float delay)
    {
        StartCoroutine(RestartScene(delay));
    }

    public IEnumerator RestartScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
