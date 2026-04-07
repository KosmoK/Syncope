using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] AnimationClip damageClip;
    [SerializeField] int frames;
    [SerializeField] int damage;
    [Header("Customize Shake, leave as -1 for none")]
    [SerializeField] int shakeFrame = -1;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeIntensity;
    [SerializeField] float shakeFrequency;
    [Header("Basic attack flags")]
    [SerializeField] bool isBasicAttack1 = false;
    [SerializeField] bool isBasicAttack2 = false;
    private int frame;
    private SpriteRenderer spriteRenderer;
    private Sprite lastSprite;
    private Animator animator;
    private int bonusFireDamage;
    private int bonusIceDamage;
    private List<string> enemyTags = new List<string>
    {
        "LavaEnemy",
        "IceEnemy",
        "Phoenix",
        "Golem"
    };

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

        if (shakeFrame == ind)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerCamera>().ShakeCamera(shakeIntensity, shakeFrequency, shakeDuration);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        string tag = gameObject.tag;
        AnimStatesBase asb;
        fetchBonusDamage();

        if (enemyTags.Contains(tag))
        {
            asb = gameObject.GetComponent<AnimStatesBase>();
        } else
        {
            return;
        }

        if (isBasicAttack1)
        {
            damage = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>().getDamage1();
        } else if (isBasicAttack2)
        {
            damage = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>().getDamage2();
        }

        if (tag == "Phoenix")
        {
            Phoenix phoenix = gameObject.GetComponent<Phoenix>();
            phoenix.dealDamage(damage+bonusFireDamage);
        } else if (tag == "Golem")
        {
            Golem golem = gameObject.GetComponent<Golem>();
            golem.dealDamage(damage+bonusIceDamage);
        } else if (tag == "Final Boss")
        {
            FinalBoss fb = gameObject.GetComponent<FinalBoss>();
            fb.dealDamage(damage);
        }

        if (asb == null || damage == -1)
        {
            return;
        }

        if (tag == "LavaEnemy")
        {
            asb.dealDamage(damage+bonusFireDamage);
        } else if (tag == "IceEnemy")
        {
            asb.dealDamage(damage+bonusIceDamage);
        } else
        {
            asb.dealDamage(damage);
        }
    }

    private void fetchBonusDamage()
    {
        DrugManager dm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DrugManager>();
        bonusFireDamage = dm.getBonusLavaDamage();
        bonusIceDamage = dm.getBonusIceDamage();
    }

}