using UnityEngine;

public class PhoenixBlockScript : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            return;
        }

        playerCamera.GetComponent<PlayerCamera>().changeSize = true;
        GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
