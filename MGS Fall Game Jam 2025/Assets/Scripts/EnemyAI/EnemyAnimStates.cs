using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimStates : MonoBehaviour
{

    EnemyMovementState movStates;
    private string enemyType;

    NavMeshAgent agent;
    Animator animator;
    PlayerMovement player;
    SpriteRenderer spriteRenderer;
    [Header("Animation clips")]

    [SerializeField] AnimationClip idle;
    [SerializeField] AnimationClip alert;
    [SerializeField] AnimationClip move;
    [SerializeField] AnimationClip attack;
    [SerializeField] AnimationClip damaged;
    [SerializeField] AnimationClip death;
    [SerializeField] AnimationClip[] extraAnims;
    [Header("Attack data")]
    [SerializeField] GameObject attackPrefab;
    [SerializeField] Vector2 attackDims;
    [SerializeField] Vector2 attackOffset;
    [SerializeField] float attackSpeed;
    [SerializeField] int attackDamage;
    [SerializeField] float attackDuration;
    [SerializeField] float damageCooldownAmount = 0.3f;
    [SerializeField] float attackCooldownAmount = 0.3f;
    private float damageCooldown = 0;
    private float attackCooldown = 0;
    private string currentAnim;
    private float animTime;
    private Vector3 lastKnownPosition;
    private Vector2 lastKnownDir;
    private string state;
    private string prevState;

    // used for ant leader sleep animation
    // would usually be done with inheritance but we don't got time for that lmao
    private float idleWaitTime = 0;

    [Header("Enemy Data")]
    [SerializeField] int hp;
    private bool dead = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; 

        animator = GetComponent<Animator>();
        movStates = GetComponent<EnemyMovementState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>();

        enemyType = movStates.getEnemyType();
        state = movStates.getState();
        prevState = movStates.getState();
        lastKnownPosition = transform.position;
        lastKnownDir = Vector2.left;
    }

    void Update()
    {
        state = movStates.getState();
        doDamageCooldown();
        doAttackCooldown();

        if (enemyType == "ember_ant_leader")
        {
            antLeaderHandler();
        } else if (enemyType == "rattlebones")
        {
            snakeHandler();
        } else if (enemyType == "ember_ant")
        {
            antHandler();
        } else if (enemyType == "sharmadillo")
        {
            sharmadilloHandler();
        }

        prevState = state;
        updateLastKnownVars();
    }

    public bool canChangeState()
    {
        if (currentAnim != attack.name || animTime >= 0.95)
        {
            return true;
        }

        return false;
    }

    private void doDamageCooldown()
    {
        if (damageCooldown <= 0)
        {
            damageCooldown = 0;
        } else
        {
            damageCooldown -= Time.deltaTime;
        }
    }

    private void doAttackCooldown()
    {
        if (attackCooldown <= 0)
        {
            attackCooldown = 0;
        } else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public float getAttackCooldown()
    {
        return attackCooldown;
    }

    private bool setAnimation(string name, bool ignoreTime)
    {
        if (!ignoreTime && animTime < 1)
        {
            return false;
        }

        animator.Play(name, -1, 0f);
        flipRenderer();
        currentAnim = name;

        return true;
    }

    private void updateLastKnownVars()
    {
        Vector3 pos = transform.position;
        Vector2 dir = new Vector2(pos.x - lastKnownPosition.x, pos.y - lastKnownPosition.y);
        lastKnownDir = dir;

        if (pos != lastKnownPosition)
        {
            
            lastKnownPosition = pos;
        }
    }

    private void flipRenderer()
    {
        if (lastKnownPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        } else if (lastKnownPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    private GameObject createAttack()
    {
        GameObject attackHitbox = Instantiate(attackPrefab, transform.position, Quaternion.identity);
        AttackEffects ae = attackHitbox.GetComponent<AttackEffects>();
        if (spriteRenderer.flipX)
        {
            ae.setDirection(Vector2.right);
        } else
        {
            ae.setDirection(Vector2.left);
        }
        ae.setDimensions(attackDims);
        ae.setOffset(attackOffset);
        ae.setSpeed(attackSpeed);
        ae.setDamage(attackDamage);
        ae.setDuration(attackDuration);

        attackCooldown = attack.length + attackCooldownAmount;

        return attackHitbox;
    }

    public void dealDamage(int damageAmount)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) || damageCooldown != 0)
        {
            return;
        }
        if (damageAmount >= hp)
        {
            hp = 0;
            setAnimation(death.name, true);
            dead = true;
        } else
        {
            hp -= damageAmount;
            damageCooldown = damageCooldownAmount+damaged.length;
            setAnimation(damaged.name, true);
        }
    }

    // ant stuff
    private void antLeaderHandler()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (dead)
        {
            if (agent.enabled == true)
            {
                agent.enabled = false;   
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) && animTime >= 1)
            {
                Destroy(gameObject);
            } else
            {
                return;
            }
        }

        if (state == "idle")
        {
            antLeaderIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            antLeaderWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            antLeaderChase();
        } else if (state == "attack")
        {
            antLeaderAttack();
        }
    }

    private void antLeaderIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }
        if (prevState != "idle")
        {
            idleWaitTime = 0;
        }

        if (idleWaitTime < 3)
        {
            setAnimation(extraAnims[0].name, false);
        } else
        {
            setAnimation(idle.name, false);    
        }
        idleWaitTime += Time.deltaTime;
        
    }
    private void antLeaderWake()
    {
        setAnimation(alert.name, true);
    }
    private void antLeaderChase()
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }
        bool success = setAnimation(move.name, false);
        flipRenderer();

        if (success)
        {
            agent.SetDestination(player.truePos);   
        }
    }
    private void antLeaderAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
        }
    }

    // regular ant stuff
    private void antHandler()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (dead)
        {
            if (agent.enabled == true)
            {
                agent.enabled = false;   
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) && animTime >= 1)
            {
                Destroy(gameObject);
            } else
            {
                return;
            }
        }

        if (state == "idle")
        {
            antIdle();
        } else if (state == "chase")
        {
            antChase();
        } else if (state == "attack")
        {
            antAttack();
        }
    }

    private void antIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }
        if (prevState != "idle")
        {
            idleWaitTime = 0;
        }

        setAnimation(idle.name, false);    

        idleWaitTime += Time.deltaTime;
        
    }
    private void antChase()
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }
        bool success = setAnimation(move.name, false);
        flipRenderer();

        if (success)
        {
            agent.SetDestination(player.truePos);   
        }
    }
    private void antAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
        }
    }


    // rattlebones stuff
    private void snakeHandler()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (dead)
        {
            if (agent.enabled == true)
            {
                agent.enabled = false;   
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) && animTime >= 1)
            {
                Destroy(gameObject);
            } else
            {
                return;
            }
        }

        if (state == "idle")
        {
            snakeIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            snakeWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            snakeChase();
        } else if (state == "attack")
        {
            snakeAttack();
        }
    }

    private void snakeIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }
        if (prevState != "idle")
        {
            idleWaitTime = 0;
        }

        if (idleWaitTime < 3)
        {
            setAnimation(extraAnims[0].name, false);
        } else
        {
            setAnimation(idle.name, false);    
        }
        idleWaitTime += Time.deltaTime;
        
    }
    private void snakeWake()
    {
        setAnimation(alert.name, true);
    }
    private void snakeChase()
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }
        
        bool success;
        if (currentAnim == alert.name)
        {
            success = setAnimation(extraAnims[1].name, false);
        } else
        {
            success = setAnimation(move.name, false);   
        }
        flipRenderer();

        if (success)
        {
            agent.SetDestination(player.truePos);   
        }
    }
    private void snakeAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
        }
    }

    private void sharmadilloHandler()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (dead)
        {
            if (agent.enabled == true)
            {
                agent.enabled = false;   
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) && animTime >= 1)
            {
                Destroy(gameObject);
            } else
            {
                return;
            }
        }

        if (state == "idle")
        {
            sharmadilloIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            sharmadilloWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            GetComponent<NavMeshAgent>().speed = 7f;
            sharmadilloChase();
        } else if (state == "attack")
        {
            GetComponent<NavMeshAgent>().speed = 12f;
            sharmadilloAttack();
        }
    }

    private void sharmadilloIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }
        if (prevState != "idle")
        {
            idleWaitTime = 0;
        }

        if (idleWaitTime < 3)
        {
            setAnimation(extraAnims[0].name, false);
        } else
        {
            setAnimation(idle.name, false);    
        }
        idleWaitTime += Time.deltaTime;
        
    }
    private void sharmadilloWake()
    {
        setAnimation(alert.name, true);
    }
    private void sharmadilloChase()
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }
        bool success = setAnimation(move.name, false);
        flipRenderer();

        if (success)
        {
            agent.SetDestination(player.truePos);   
        }
    }
    private void sharmadilloAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            GameObject atk = createAttack();
            atk.GetComponent<AttackEffects>().setSharmadilloAttack(gameObject);
        }
    }

}
