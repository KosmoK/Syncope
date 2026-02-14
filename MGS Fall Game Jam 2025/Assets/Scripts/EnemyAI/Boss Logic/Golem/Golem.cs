using System;
using Unity.VisualScripting;
using UnityEngine;

public class Golem : MonoBehaviour
{
    GameManager gm;
    GameObject player;
    PlayerMovement playerMovement;
    Movement entityMovement;
    HazardDamager hazardDamager;
    Animator animator;
    Rigidbody2D rb;
    BoxCollider2D bounceCollider;
    
    [Header("Animation Clips")]
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] AnimationClip alertAnim;
    [SerializeField] AnimationClip hurtAnim;
    [SerializeField] AnimationClip moveAnim;
    [SerializeField] AnimationClip spikeAttackAnim;
    [SerializeField] AnimationClip death;
    
    [Header("Attack Info")]
    [SerializeField] GameObject iciclePrefab;
    [SerializeField] float spinSpeedP1;
    [SerializeField] float spinSpeedP2;
    [SerializeField] float spinLen;
    [SerializeField] float spinSpeedIncP1;
    [SerializeField] float spinSpeedIncP2;
    // phoenix vars
    [SerializeField] float timeBetweenP1Attacks;
    [SerializeField] float timeBetweenP2Attacks;
    [SerializeField] float idleTimeP1;
    [SerializeField] float idleTimeP2;
    [SerializeField] float icicleSpeedP1;
    [SerializeField] float icicleSpeedP2;
    [SerializeField] float icicleAttackTime;
    [SerializeField] int spinDamage;
    private Vector2 spinDir;
    private float spinSpeed;
    private float spinTime;

    // phoenix vars
    private float timeSinceLastAttack;
    private float idleTime;
    private int numAttacks;
    private string state;

    private float animTime;
    private bool phase1 = true;
    public float hp = 80;
    private float phase2HpThreshold = 40;
    private float damageCooldown;
    private float icicleCooldown;
    private float icicleBurstCooldown;
    private float icicleBurstIndCooldown;
    private float icicleBurstTime;
    private Vector3 icicleBurstVector;
    private int icicleNum;
    private int icicleLayerMask;

    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerMovement = player.GetComponent<PlayerMovement>();
        entityMovement = player.GetComponent<Movement>();
        animator = GetComponent<Animator>();
        hazardDamager = GetComponent<HazardDamager>();
        rb = GetComponent<Rigidbody2D>();
        
        foreach (Transform t in transform)
        {
            bounceCollider = t.gameObject.GetComponent<BoxCollider2D>();
        }

        spinDir = Vector2.zero;

        state = "beforeBattle";
        initIcicleBurst();
    }

    void Update()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        doDamageCooldown();

        if (hp <= phase2HpThreshold && phase1)
        {
            phase1 = false;
            state = "phase2Start";
        }

        stateMachine();
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

    
    private void hazardDamageModifier(bool enabled)
    {
        if (enabled)
        {
            hazardDamager.doesDamage = true;
            hazardDamager.knockBackStrength = 0.1f;
        } else
        {
            hazardDamager.doesDamage = false;
            hazardDamager.knockBackStrength = 0f;
        }
    }

    public void dealDamage(int damageAmount)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) || animator.GetCurrentAnimatorStateInfo(0).IsName(alertAnim.name) || animator.GetCurrentAnimatorStateInfo(0).IsName(moveAnim.name) || damageCooldown != 0)
        {
            return;
        }
        if (damageAmount >= hp)
        {
            hp = 0;
            setAnimation(death.name, true);
            state = "dead";
        } else
        {
            hp -= damageAmount;
            damageCooldown = hurtAnim.length*0.75f;
            setAnimation(hurtAnim.name, true);   
        }
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

    void stateMachine()
    {
        if (state == "beforeBattle")
        {
            beforeBattleFunc();
        } else if (state == "blowBack")
        {
            blowBackFunc();
            numAttacks = UnityEngine.Random.Range(1, 3);
        } else if (state == "spinAttack")
        {
            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
            spinDir = (playerMovement.truePos - myPos).normalized;
            spinAttackFunc();
        } else if (state == "idle")
        {
            if (rb.linearVelocity == Vector2.zero)
            {
                bounceCollider.enabled = false;
            }   
            idleFunc();
        } else if (state == "phase2Start")
        {
            phase2StartFunc();
        } else if (state == "icicleAttack")
        {
            icicleNum = 0;
            icicleAttackFunc();
        } else if (state == "icicleBurst")
        {
            icicleBurstFunc();
        } else if (state == "dead")
        {
            rb.linearVelocity = Vector2.zero;
            if (animTime > 1  && animator.GetCurrentAnimatorStateInfo(0).IsName(death.name))
            {
                gm.golemDefeated = true;
                gm.sceneTransition = true;
                Destroy(gameObject);
            }  else if (!animator.GetCurrentAnimatorStateInfo(0).IsName(death.name))
            {
                setAnimation(death.name, true);
            }
        }
    }

    // void setState(string state)
    // {
    //     if (state == "idle")
    //     {
    //         idleTime = 0;
    //         return;
    //     }
    // }

    private void beforeBattleFunc()
    {
        float dist = getPlayerDist();
        if (dist != -1 && dist < 15)
        {
            setAnimation(alertAnim.name, true);
            state = "blowBack";
        } else
        {
            setAnimation(idleAnim.name, false);
        }
    }

    private void blowBackFunc()
    {
        if (animTime > 0.5)
        {
            bounceCollider.enabled = false;
            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
            entityMovement.velocity = 0.1f * (playerMovement.truePos - myPos).normalized;
            state = "spinAttack";
            if (phase1)
            {
                spinSpeed = spinSpeedP1;
            } else
            {
                spinSpeed = spinSpeedP2;
            }
            spinTime = 0f;
            rb.linearDamping = 0;
        }
    }

    private void spinAttackFunc()
    {
        setAnimation(moveAnim.name, false);
        
        if (rb.linearVelocity == Vector2.zero && animator.GetCurrentAnimatorStateInfo(0).IsName(moveAnim.name))
        {
            bounceCollider.enabled = true;
            rb.linearVelocity = spinDir * spinSpeed;
        }

        if (phase1)
        {
            rb.linearVelocity += rb.linearVelocity.normalized * spinSpeedIncP1*Time.deltaTime;
        } else
        {
            rb.linearVelocity += rb.linearVelocity.normalized * spinSpeedIncP2*Time.deltaTime;
        }

        if (spinTime > spinLen)
        {
            state = "idle";
            rb.linearDamping = 5f;
            idleTime = 0;
            return;
        }

        spinTime += Time.deltaTime;
    }

    private void idleFunc()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(idleAnim.name))
        {
            setAnimation(idleAnim.name, false);
        }

        if (phase1 && idleTime > idleTimeP1)
        {
            chooseAttackState();
        } else if (!phase1 && idleTime > idleTimeP2)
        {
            chooseAttackState();
        }

        idleTime += Time.deltaTime;
    }

    private void chooseAttackState()
    {
        int attack = UnityEngine.Random.Range(0, 3);
        attack = 2;

        if (attack == 0)
        {
            spinTime = 0;
            rb.linearDamping = 0;
            state = "spinAttack";
        } else if (attack == 1)
        {
            state = "icicleAttack";
        } else if (attack == 2)
        {
            initIcicleBurst();
            state = "icicleBurst";
        }
    }

    private void phase2StartFunc()
    {
        setAnimation(alertAnim.name, true);
        state = "blowBack";
    }

    private void icicleAttackFunc()
    {
        if (icicleCooldown <= 0)
        {
            Vector3 positionCorrection = new Vector3(0, -0.9f, 0);
            GameObject g = Instantiate(iciclePrefab, player.transform.position + positionCorrection, Quaternion.identity);
            g.GetComponent<GolemIciclePrecursor>().setRenderLayer(icicleNum);
            icicleNum += 2;
            if (phase1)
            {
                icicleCooldown = icicleSpeedP1;
            } else
            {
                icicleCooldown = icicleSpeedP2;
            }
        }

        icicleCooldown -= Time.deltaTime;
    }

    // private float icicleBurstCooldown;       cooldown between bursts
    // private float icicleBurstIndCooldown;    cooldown between individual icicles
    // private float icicleBurstTime;           time spent on current burst ()
    // private Vector3 icicleBurstVector;     location we're aiming for

    private void initIcicleBurst()
    {
        icicleBurstTime = 0;
        icicleBurstIndCooldown = 0;
        icicleNum = 0;
        icicleLayerMask = LayerMask.GetMask("IceBossPong");

        icicleBurstVector = player.transform.position - transform.position + Vector3.down;
        icicleBurstVector = icicleBurstVector.normalized * 45f;
    }
    private void icicleBurstFunc()
    {
        float burstDuration = 1f;
        float maxBurstOffset = 2f;
        float maxScale = 4.5f;

        float t = Math.Max(0, Math.Min(1, icicleBurstTime/burstDuration));
        Vector3 position = icicleBurstVector.normalized * icicleBurstVector.magnitude * t;
        Vector3 offsetVector = new Vector3(-icicleBurstVector.normalized.y, icicleBurstVector.normalized.x, 0) * t * maxBurstOffset * UnityEngine.Random.value;
        offsetVector = (UnityEngine.Random.Range(0, 2) == 0) ? offsetVector : -offsetVector;
        
        position += transform.position + offsetVector;

        if (icicleBurstIndCooldown <= 0)
        {
            GameObject g = Instantiate(iciclePrefab, position, Quaternion.identity);
            GolemIciclePrecursor gi = g.GetComponent<GolemIciclePrecursor>();
            gi.setScaleFactor(maxScale * t);
            gi.setRenderLayer(icicleNum);
            // gi.setSpawnIcicleTime(0);
            icicleBurstIndCooldown = 0.05f;

            icicleNum++;
        }

        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(myPos, position - transform.position, (position-transform.position).magnitude, icicleLayerMask);
        if (icicleBurstTime >= burstDuration || hit)
        {
            state = "idle";
            // idleTime = 0;
            // return;
        }

        icicleBurstTime += Time.deltaTime;
        icicleBurstIndCooldown -= Time.deltaTime;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && state == "spinAttack")
        {
            EntityStatus es = collision.gameObject.GetComponent<EntityStatus>();
            es.Hurt(spinDamage);
        } else
        {
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int targetPlayer = UnityEngine.Random.Range(0, 3);
        if (targetPlayer != 0)
        {
            return;
        }

        float mag = rb.linearVelocity.magnitude;
        rb.linearVelocity = spinDir * mag;
    }

}
