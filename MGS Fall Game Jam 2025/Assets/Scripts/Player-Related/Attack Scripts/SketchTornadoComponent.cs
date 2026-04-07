using UnityEngine;
using UnityEngine.AI;

public class SketchTornadoComponent : MonoBehaviour
{
    private float time;
    void Start()
    {
        GetComponent<AnimStatesBase>().setSketchTornado(true);
        GetComponent<NavMeshAgent>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<AnimStatesBase>().setSketchTornado(false);
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
