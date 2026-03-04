using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class AnimStatesBase : MonoBehaviour
{

    [Serializable]
    protected class SoundEntry
    {
        [SerializeField] string name;
        [SerializeField] string atlasName;

        public SoundEntry(string name)
        {
            this.name = name;
        }
        public string getName()
        {
            return name;
        }
        public string getAtlasName()
        {
            return atlasName;
        }
    }

    [SerializeField] protected List<SoundEntry> sounds = new List<SoundEntry> {new SoundEntry("AttackSfx"), new SoundEntry("DeathSfx"), new SoundEntry("WalkSfx"), new SoundEntry("DamagedSfx")};

    EnemyMovementState movStates;
    protected string enemyType;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected PlayerMovement player;
    protected GameObject gameManager;
    protected AudioSource audioSource;
    protected SoundAtlas soundAtlas;
    SpriteRenderer spriteRenderer;
    [Header("Coin stuff")]
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float coinAnimMaxX;
    [SerializeField] float coinAnimMaxY;
    [SerializeField] float coinAnimDuration;
    [SerializeField] Vector2 coinMinMax;
    [SerializeField] Vector2 coinValue;
    
    [Header("Animation clips")]

    [SerializeField] protected AnimationClip idle;
    [SerializeField] protected AnimationClip alert;
    [SerializeField] protected AnimationClip move;
    [SerializeField] protected AnimationClip attack;
    [SerializeField] protected AnimationClip damaged;
    [SerializeField] protected AnimationClip death;
    [Header("Attack data")]
    [SerializeField] protected GameObject attackPrefab;
    [SerializeField] protected float damageCooldownAmount = 0.3f;
    [SerializeField] protected float attackCooldownAmount = 0.3f;
    // test
    protected float damageCooldown = 0;
    protected float attackCooldown = 0;
    protected string currentAnim;
    protected float animTime;
    protected Vector3 lastKnownPosition;
    protected Vector2 lastKnownDir;
    protected string state;
    protected string prevState;

    protected float idleWaitTime = 0;
    [Header("Enemy Data")]
    [SerializeField] int hp;
    protected bool dead = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; 

        animator = GetComponent<Animator>();
        movStates = GetComponent<EnemyMovementState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        audioSource = GetComponent<AudioSource>();
        soundAtlas = gameManager.GetComponent<SoundAtlas>();

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

        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (dead)
        {
            if (agent.enabled == true)
            {
                agent.enabled = false;   
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) && animTime >= 1)
            {
                spawnCoins();
                Destroy(gameObject);
            } else
            {
                return;
            }
        }

        animHandler();

        prevState = state;
        updateLastKnownVars();
    }

    private void spawnCoins()
    {
        int coinNum = (int) UnityEngine.Random.Range(coinMinMax.x, coinMinMax.y);
        int parity = UnityEngine.Random.Range(0, 1);
        for (int i = 0; i < coinNum; i++)
        {
            Coin c = Instantiate(coinPrefab, transform.position, Quaternion.identity).GetComponent<Coin>();
            Vector2 dir = new Vector2(UnityEngine.Random.Range(1, coinAnimMaxX), UnityEngine.Random.Range(1, coinAnimMaxY));
            dir.x *= ((i+parity)%2 == 0) ? 1 : -1;
            c.SetValsAndLenAndAmount(dir, coinAnimDuration*UnityEngine.Random.Range(0.5f, 1f), (int) UnityEngine.Random.Range(coinValue.x, coinValue.y));
        }
    }

    protected virtual void animHandler() { Debug.Log("implement handler"); }

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

    protected bool setAnimation(string name, bool ignoreTime, string sfx = "")
    {
        if (!ignoreTime && animTime < 1)
        {
            return false;
        }

        animator.Play(name, -1, 0f);
        flipRenderer();
        currentAnim = name;

        if (sfx != "")
        {
            playSound(sfx);
        }

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

    protected void flipRenderer()
    {
        if (lastKnownPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        } else if (lastKnownPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    protected GameObject createAttack()
    {
        GameObject attackHitbox = Instantiate(attackPrefab, transform);
        // attackHitbox.transform.localScale = transform.localScale; don't need, forgot how parents work lmfaooo

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
            setAnimation(death.name, true, "DeathSfx");
            dead = true;
        } else
        {
            hp -= damageAmount;
            damageCooldown = damageCooldownAmount+damaged.length;
            setAnimation(damaged.name, true, "DamagedSfx");
        }
    }

    protected void playSound(string sound, float volume = 1)
    {
        string atlasName = "";
        foreach (SoundEntry soundEntry in sounds)
        {
            if (soundEntry.getName() == sound)
            {
                atlasName = soundEntry.getAtlasName();
                break;
            }
        }

        AudioClip clip = soundAtlas.GetClip(atlasName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        } else
        {
            Debug.LogError($"No clip found with name: {sound}");
        }
    }

}
