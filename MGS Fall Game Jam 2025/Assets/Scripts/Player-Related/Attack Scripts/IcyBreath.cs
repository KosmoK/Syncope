using UnityEngine;

public class IcyBreath : MonoBehaviour
{
    [SerializeField] bool p1;
    [SerializeField] bool p2;
    [SerializeField] GameObject p1Prefab;
    [SerializeField] GameObject p2Prefab;
    [SerializeField] float lingerDuration;
    [SerializeField] float slowDuration;

    void OnDestroy()
    {
        if (p1)
        {
            GameObject g = Instantiate(p1Prefab);
            g.transform.position = transform.position;
            if (transform.lossyScale.x < 0)
            {
                g.transform.localScale = new Vector3(-1, 1, 1);
            }
            g.GetComponent<IcyBreath>().setLingerDuration(lingerDuration);
            g.GetComponent<IcyBreath>().setSlowDuration(slowDuration);

            return;
        }

        if (p2)
        {
            GameObject g = Instantiate(p2Prefab);
            g.transform.position = transform.position;
            if (transform.lossyScale.x < 0)
            {
                g.transform.localScale = new Vector3(-1, 1, 1);
            }
            g.GetComponent<IcyBreathLinger>().setLingerDuration(lingerDuration);
            g.GetComponent<IcyBreathLinger>().setSlowDuration(slowDuration);

            return;
        }
    }

    public void setLingerDuration(float ld)
    {
        lingerDuration = ld;
    }

    public void setSlowDuration(float sd)
    {
        slowDuration = sd;
    }
}
