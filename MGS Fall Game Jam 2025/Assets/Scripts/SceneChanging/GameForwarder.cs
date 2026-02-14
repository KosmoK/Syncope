using UnityEngine;
using UnityEngine.SceneManagement;

public class GameForwarder : MonoBehaviour
{
    GameManager gm;
    string scene;
    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();
        scene = SceneManager.GetActiveScene().name;
    }

    public void setDrug(string newDrug)
    {
        if (scene == "drugUI")
        {
            gm.drug = newDrug;
            gm.sceneTransition = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
