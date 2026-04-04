// pls work
using UnityEngine;

public class GravityBall : MonoBehaviour
{ 
    [SerializeField] Transform ballMiddle;
    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (!(gameObject.tag == "LavaEnemy" || gameObject.tag == "IceEnemy"))
        {
            return;
        }

        GravityBallComponent gbc = gameObject.AddComponent<GravityBallComponent>();
        
        Vector3 dir = rb.transform.position - ballMiddle.position;
        float amount = rb.mass*rb.linearDamping*dir.magnitude/2; // makes it so that it travels exactly to the middle
        Vector3 forceVec = dir.normalized * amount;
        rb.AddForce(forceVec, ForceMode2D.Impulse);
        
    }
}
