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
    private bool appliedDrugs = false;

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
        if ((scene == "Level1Lava" || scene == "Level1Ice") && GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            SceneManager.LoadScene("drugUI");
        }

        if (scene == "Level1Lava" || scene == "Level1Ice")
        {
            applyDrugs();
        }

        // Debug.Log($"{sceneTransition} {fadingScreen}");
        if (!sceneTransition || fadingScreen)
        {
            return;
        }

        sceneTransition = false;
        if (scene == "Titlescreen")
        {
            StartCoroutine(fadeToScene("drugUI", 1f));  
        } else if (scene == "drugUI")
        {
            appliedDrugs = false;
            StartCoroutine(fadeToScene("Level1Transition"));
        } else if (scene == "Level1Transition")
        {
            StartCoroutine(fadeToScene(level1Transition));
        } else if (scene == "Level1Lava")
        {
            StartCoroutine(fadeToScene("level1Transition"));
        } else if (scene == "Level1Ice")
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

    private void applyDrugs()
    {
        if (appliedDrugs)
        {
            return;
        }
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];

        if (drug == "velocity")
        {
            player.GetComponent<Movement>().topSpeed = 7f;
        } else if (drug == "dash")
        {
            player.GetComponent<PlayerMovement>().dashStrength = 0.05f;
        } else if (drug == "fire")
        {
            player.GetComponent<PlayerAttack>().bonusIceDamage = 1;
        } else if (drug == "ice")
        {
            player.GetComponent<PlayerAttack>().bonusLavaDamage = 1;
        }

        appliedDrugs = true;
    }

}
