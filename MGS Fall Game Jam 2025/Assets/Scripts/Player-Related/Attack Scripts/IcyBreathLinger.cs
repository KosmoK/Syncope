using UnityEngine;

public class IcyBreathLinger : MonoBehaviour
{
    [SerializeField] AnimationClip damageClip;
    private Animator animator;
    float t;
    private float lingerDuration;
    private float slowDuration;

    void Start()
    {
        animator = transform.GetChild(transform.childCount-1).GetComponent<Animator>();
        animator.Play(damageClip.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (t > lingerDuration)
        {
            Destroy(gameObject);
        }

        t += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (!(gameObject.tag == "LavaEnemy" || gameObject.tag == "IceEnemy"))
        {
            return;
        }

        IcyBreathComponent ibc = gameObject.AddComponent<IcyBreathComponent>();
        ibc.setDuration(slowDuration);
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
