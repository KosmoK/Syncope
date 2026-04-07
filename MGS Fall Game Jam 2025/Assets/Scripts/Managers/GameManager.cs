using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private ScreenFader screenFader;
    private bool fadingScreen = false;
    public bool sceneTransition;
    public string scene = "Titlescreen";
    public string drug;
    public string level1Transition;
    public bool phoenixDefeated = false;
    public bool golemDefeated = false;
    public bool playerDied = false;
    private bool startButtonClicked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        scene = SceneManager.GetActiveScene().name;

        sceneStateMachine();
    }

    private void sceneStateMachine()
    {
        if ((scene == "L1Lava" || scene == "L1Ice") && GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            StartCoroutine(fadeToScene("Level1Transition"));
        }

        // Debug.Log($"{sceneTransition} {fadingScreen}");
        if (fadingScreen)
        {
            return;
        }

        // sceneTransition = false;
        if (scene == "Titlescreen" && startButtonClicked)
        {
            StartCoroutine(fadeToScene("Level1Transition"));  
        } else if (scene == "Level1Transition" && sceneTransition)
        {
            sceneTransition = false;
            if (level1Transition == "Level1Lava")
            {
                StartCoroutine(fadeToScene("L1Lava"));
            } else
            {
                StartCoroutine(fadeToScene("L1Ice"));
            }
            // StartCoroutine(fadeToScene(level1Transition));
        } else if (scene == "Level1Lava" && sceneTransition)
        {
            StartCoroutine(fadeToScene("level1Transition"));
        } else if (scene == "Level1Ice" && sceneTransition)
        {
            StartCoroutine(fadeToScene("level1Transition"));
        }
    }

    private IEnumerator fadeToScene(string loadScene, float fadeDuration = 0.5f)
    {
        fadingScreen = true;
        screenFader.setFadeDuration(fadeDuration);
        yield return StartCoroutine(screenFader.fadeIn());

        SceneManager.LoadScene(loadScene);
        yield return StartCoroutine(screenFader.fadeOut());
        fadingScreen = false;
    }

    public void clickStartButton()
    {
        startButtonClicked = true;
    }

}
