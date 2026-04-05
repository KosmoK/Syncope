using UnityEngine;

public class SketchTornado : MonoBehaviour
{
    [SerializeField] float stunDuration;
    [SerializeField] float lingerDuration;
    [SerializeField] GameObject lingerPrefab;

    void OnDestroy()
    {
        GameObject g = Instantiate(lingerPrefab);
        g.transform.position = transform.position;
        SketchTornadoLinger stl = g.GetComponent<SketchTornadoLinger>();
        stl.setStunDuration(stunDuration);
        stl.setLingerDuration(lingerDuration);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (!(gameObject.tag == "LavaEnemy" || gameObject.tag == "IceEnemy"))
        {
            return;
        }

        SketchTornadoComponent stc = gameObject.AddComponent<SketchTornadoComponent>();
        stc.setDuration(stunDuration);
    }
}
