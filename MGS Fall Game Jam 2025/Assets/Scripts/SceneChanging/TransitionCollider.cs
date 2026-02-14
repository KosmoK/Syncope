using UnityEngine;

public class TransitionCollider : MonoBehaviour
{
    GameManager gm;
    [SerializeField] string nextScene;
    
    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();    
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(collision.gameObject);
            gm.level1Transition = nextScene;
            gm.sceneTransition = true;
        }
    }
}
