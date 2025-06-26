using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedAudioClip
{
    public string key;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;

    public string groupTag;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public List<NamedAudioClip> audioClips = new();

    private Dictionary<string, NamedAudioClip> clipDict = new();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        foreach (var item in audioClips)
        {
            if (!clipDict.ContainsKey(item.key) && item.clip != null)
                clipDict.Add(item.key, item);
        }
    }

    public void Play(string key)
    {
        if (clipDict.TryGetValue(key, out var entry))
        {
            audioSource.PlayOneShot(entry.clip, entry.volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager: No clip found with key '{key}'");
        }
    }

    public void PlayRandomFromGroup(string groupTag)
    {
        var matches = audioClips.FindAll(c => c.groupTag == groupTag && c.clip != null);
        if (matches.Count > 0)
        {
            var chosen = matches[Random.Range(0, matches.Count)];
            audioSource.PlayOneShot(chosen.clip, chosen.volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager: No clips found in group '{groupTag}'");
        }
    }
}
