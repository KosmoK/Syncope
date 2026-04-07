using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    GameManager gm;
    GameObject player;
    Animator animator;
    PlayerMovement playerMovement;
    Movement entityMovement;
    SpriteRenderer sr;
    [Header("Animation Clips")]
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] AnimationClip entranceAnim;
    [SerializeField] AnimationClip deathAnim;
    [SerializeField] AnimationClip distortionAnim;
    [SerializeField] AnimationClip hurtAnim;
    [SerializeField] AnimationClip screamAnim;
    [SerializeField] AnimationClip swipeAnim;
    [SerializeField] AnimationClip walkAnim;

    private string state;
    private float animTime;
    private bool phase1 = true;
    public float hp = 80;
    private float phase2HpThreshold = 40;
    private float damageCooldown;
    private float idleTime;

    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerMovement = player.GetComponent<PlayerMovement>();
        entityMovement = player.GetComponent<Movement>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        animator.enabled = false;
        sr.enabled = false;

        state = "waiting";
    }

    // Update is called once per frame
    void Update()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (hp <= phase2HpThreshold && phase1)
        {
            phase1 = false;
            state = "phase2Start";
        }

        stateMachine();
    }

    private void stateMachine()
    {
        if (state == "waiting")
        {
            return;
        } else if (state == "entrance")
        {
            animator.enabled = true;
            sr.enabled = true;
            setAnimation(entranceAnim.name, false);
            state = "chase";
        }
    }

    private float getPlayerDist()
    {
        if (player == null)
        {
            return -1;
        }
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(myPos, playerMovement.truePos - myPos);
        Debug.DrawRay(myPos, playerMovement.truePos - myPos);

        if (hit && hit.transform.gameObject.name == "Player")
        {
            return hit.distance;
        }

        return -1;
    }

    public void dealDamage(int damageAmount)
    {
        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName(idleAnim.name) && damageCooldown != 0))
        {
            return;
        }

        if (damageAmount >= hp)
        {
            hp = 0;
            setAnimation(deathAnim.name, true);
            state = "dead";
        } else
        {
            hp -= damageAmount;
            damageCooldown = 0.4f;
            setAnimation(hurtAnim.name, true);   
        }
    }

    private bool setAnimation(string name, bool ignoreTime)
    {
        // Debug.Log(name);
        if (!ignoreTime && animTime < 1)
        {
            return false;
        }

        animator.Play(name, -1, 0f);

        return true;
    }

    public void startFight()
    {
        Debug.Log("Called");
        if (state == "waiting")
        {
            state = "entrance";
        }
    }
}
