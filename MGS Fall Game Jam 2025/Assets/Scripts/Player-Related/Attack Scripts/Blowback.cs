using UnityEngine;

public class Blowback : MonoBehaviour
{
    [SerializeField] float amount;

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (gameObject.tag == "Player" || rb == null)
        {
            return;
        }

        Vector3 vec = gameObject.transform.position - transform.position;
        rb.AddForce(vec * amount, ForceMode2D.Impulse);
    }
}
