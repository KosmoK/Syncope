using UnityEngine;

public class Level1Doors : MonoBehaviour
{
    [SerializeField] Sprite fullLock;
    [SerializeField] Sprite iceLock;
    [SerializeField] Sprite lavaLock;
    [SerializeField] Sprite unlock;
    SpriteRenderer sr;
    GameManager gm;

    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();
        sr = GetComponent<SpriteRenderer>();

        if (gm.golemDefeated && gm.phoenixDefeated)
        {
            sr.sprite = unlock;
            transform.localScale = new Vector3(7f, 7f, 1f);
        } else if (gm.golemDefeated)
        {
            sr.sprite = lavaLock;
        } else if (gm.phoenixDefeated)
        {
            sr.sprite = iceLock;
        } else
        {
            sr.sprite = fullLock;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || !(sr.sprite == unlock))
        {
            return;
        }

        gm.level1Transition = "Endscreen";
        gm.sceneTransition = true;
    }
}
