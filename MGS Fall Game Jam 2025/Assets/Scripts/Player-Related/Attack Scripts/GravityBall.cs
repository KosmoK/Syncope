using UnityEngine;

public class GravityBall : MonoBehaviour
{ 
    [SerializeField] Transform ballMiddle;
    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (!(gameObject.tag == "LavaEnemy" || gameObject.tag == "IceEnemy"))
        {
            return;
        }

        GravityBallComponent gbc = gameObject.AddComponent<GravityBallComponent>();
        
        // Vector3 dir = ballMiddle.position - rb.transform.position;
        // float amount = rb.mass*rb.linearDamping*dir.magnitude/2; // makes it so that it travels exactly to the middle
        // Vector3 forceVec = dir.normalized * amount;
        // Debug.Log($"amount: {forceVec}");
        // rb.AddForce(forceVec, ForceMode2D.Impulse);

        gbc.setDuration(1);
        gbc.setGravitySource(ballMiddle);
        gbc.setGravity(10);
    }
}
