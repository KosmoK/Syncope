using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] AnimationClip damageClip;
    [SerializeField] int frames;
    [SerializeField] int damage;
    private int frame;
    private SpriteRenderer spriteRenderer;
    private Sprite lastSprite;
    private Animator animator;

    public void setDamage(int d)
    {
        damage = d;
    }

    void Start()
    {
        spriteRenderer = transform.GetChild(transform.childCount-1).GetComponent<SpriteRenderer>();
        lastSprite = spriteRenderer.sprite;

        animator = transform.GetChild(transform.childCount-1).GetComponent<Animator>();
        animator.Play(damageClip.name);

        setCollider(0);
    }

    void Update()
    {
        if (spriteRenderer.sprite != lastSprite)
        {
            frame++;
            setCollider(frame);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99)
        {
            Destroy(gameObject);
        }

        lastSprite = spriteRenderer.sprite;
    }

    void setCollider(int ind)
    {
        for (int i = 0; i < frames; i++)
        {
            Transform t = transform.GetChild(i);
            t.gameObject.SetActive((i == ind) ? true : false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        EntityStatus es;

        if (gameObject.tag == "Player")
        {
            es = gameObject.GetComponent<EntityStatus>();
        } else
        {
            return;
        }

        es.Hurt(damage);
    }

}
