using UnityEngine;

public class SketchTornadoLinger : MonoBehaviour
{
    [SerializeField] AnimationClip damageClip;
    private Animator animator;
    private float stunDuration;
    float t;
    private float lingerDuration;
    private Transform player;

    void Start()
    {
        animator = transform.GetChild(transform.childCount-1).GetComponent<Animator>();
        animator.Play(damageClip.name);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (t > lingerDuration)
        {
            Destroy(gameObject);
        }

        transform.position = player.position;

        t += Time.deltaTime;
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

    public void setStunDuration(float t)
    {
        stunDuration = t;
    }
    public void setLingerDuration(float ld)
    {
        lingerDuration = ld;
    }
}
