using UnityEngine;

public class BackgroundMusicPersistence : MonoBehaviour
{
    private static BackgroundMusicPersistence instance;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 90;
    }

    public static void MuteMusic()
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.volume = 0f;
        }
    }

    public static void UnmuteMusic()
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.volume = 1f;
        }
    }
}