using UnityEngine;
using UnityEngine.AI;

public class IcyBreathComponent : MonoBehaviour
{
    private float time;
    private float origSpeed;
    NavMeshAgent nma;
    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        origSpeed = nma.speed;
        nma.speed = origSpeed/2;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            nma.speed = origSpeed;
            Destroy(this);
        }
    }

    public void setDuration(float d)
    {
        if (d < 0)
        {
            Debug.LogError("Invalid sketch tornado duration {d}");
        }

        time = d;
    }
}
