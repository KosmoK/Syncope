using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Events")]
    public EventReference menuSong;
    public EventReference level1Ice;
    public EventReference level1Fire;

    private EventInstance musicInstance;

    
    void Awake()
    {
        Debug.Log("Anyone there? 1");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Titlescreen":
                PlayMusic(menuSong);
                break;

            case "L1Ice":
                PlayMusic(level1Ice);
                break;

            case "L1Lava":
                PlayMusic(level1Fire);
                Debug.Log("Anyone there? 2");
                break;
        }
    }

    public void PlayMusic(EventReference musicEvent)
    {
        // Stop current music
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    public void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }
}