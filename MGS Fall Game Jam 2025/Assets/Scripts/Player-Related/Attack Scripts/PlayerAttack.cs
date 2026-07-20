using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public PlayerMovement playerMovement;
    public float knockBackStrength = 10f;
    public float knockBackTime = 0.5f;
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
        GameObject collidedGameObject = collision.gameObject;
        string tag = collidedGameObject.tag;
        AnimStatesBase asb;
        fetchBonusDamage();

        if (enemyTags.Contains(tag))
        {
            asb = collidedGameObject.GetComponent<AnimStatesBase>();
        } else
        {
            return;
        }

        if (isBasicAttack1)
        {
            Debug.Log("Basic attack done on " + collidedGameObject.name); // Seems to work perfectly fine in here
            damage = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>().getDamage1();
        } else if (isBasicAttack2)
        {
            damage = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>().getDamage2();
        }
        // Knockback Scripting
        collidedGameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D enemyRB);
        collidedGameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent enemyNavAgent);

        if (enemyRB != null && enemyNavAgent != null)
        {
            StartCoroutine(KnockBackCoroutine(enemyRB, enemyNavAgent, collidedGameObject));
            // GameObject gameObject = collision.gameObject;
            // Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            // if (gameObject.tag == "Player" || rb == null)
            // // {
            // //     return;
            // // }
            // Vector3 vec = gameObject.transform.position - transform.position;
            // vec = vec.normalized;
            // rb.AddForce(vec * knockBackStrength, ForceMode2D.Impulse); // DOESNT WORK
        }

        // Boss Attacks
        if (tag == "Phoenix")
        {
            Phoenix phoenix = collidedGameObject.GetComponent<Phoenix>();
            phoenix.dealDamage(damage+bonusFireDamage);
        } else if (tag == "Golem")
        {
            Golem golem = collidedGameObject.GetComponent<Golem>();
            golem.dealDamage(damage+bonusIceDamage);
        } else if (tag == "Final Boss")
        {
            FinalBoss fb = collidedGameObject.GetComponent<FinalBoss>();
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

    IEnumerator KnockBackCoroutine(Rigidbody2D enemyRB, NavMeshAgent enemyNavAgent, GameObject collidedGameObject)
    {
        Debug.Log("Knockback applied to " + collidedGameObject.name);
        // enemyRB.AddForce(playerMovement.GetLastKnownDirection() * knockBackStrength, ForceMode2D.Impulse);
        // collidedGameObject.transform.position += new Vector3(5, 0, 0); // Of course this doesnt work cuz it ignores collisions
        
        enemyNavAgent.enabled = false;

        //yield return new WaitForSeconds(0.1f); // Wait a bit to ensure the nav agent is disabled before applying force (DOESNT WORK)
        // FOR SOME REASON IT ISNT APPLYING FORCE?
        enemyRB.AddForce(new Vector2(1, 0) * knockBackStrength, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackTime);
        enemyNavAgent.enabled = true;
        Debug.Log("KB ended");
    }

    // private bool canMove(Vector2 direction)
    // { // Thank you goat : https://www.youtube.com/watch?v=05eWA0TP3AA
    // // Checks if you are allowed to move. Shrimple as that.
    //     int count = rb.Cast(
    //         direction,
    //         movementFilter,
    //         castCollisions,
    //         direction.magnitude //topSpeed * Time.fixedDeltaTime + collisionOffset
    //         );
        
    //     if(count == 0) {return true;} // If you aren't colliding with anything, you're allowed to move.
    //     else{return false;}
    // }
}