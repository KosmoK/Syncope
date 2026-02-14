using UnityEngine;

public class GolemBlock : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject golemBounds;

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            return;
        }

        playerCamera.GetComponent<PlayerCamera>().changeSize = true;
        golemBounds.SetActive(true);
    }
}
