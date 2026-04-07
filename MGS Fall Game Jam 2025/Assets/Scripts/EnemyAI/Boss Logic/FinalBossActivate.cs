using UnityEngine;

public class FinalBossActivate : MonoBehaviour
{
    [SerializeField] FinalBoss fb;
    [SerializeField] GameObject blockExit;

    void OnTriggerStay2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;

        if (gameObject.tag != "PlayerCollider")
        {
            return;
        }

        fb.startFight();
        blockExit.SetActive(true);
        Destroy(this.gameObject);
    }
}
