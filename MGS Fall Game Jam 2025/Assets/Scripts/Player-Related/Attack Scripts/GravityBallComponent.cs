using UnityEngine;
using UnityEngine.AI;

public class GravityBallComponent : MonoBehaviour
{
    private float t = 0;
    private float duration;

    void Start()
    {
        duration = 1/GetComponent<Rigidbody2D>().linearDamping;
        GetComponent<NavMeshAgent>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > duration)
        {
            GetComponent<NavMeshAgent>().enabled = true;
            Destroy(this);
        }
    }
}
