using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Phoenix : MonoBehaviour
{
    GameObject player;
    PlayerMovement playerMovement;
    Movement entityMovement;
    HazardDamager hazardDamager;
    GameManager gm;
    // [SerializeField] GameObject phoenixBlock;
    // [SerializeField] GameObject playerCamera;
    
    [Header("Animation Clips")]
    [SerializeField] AnimationClip phase1Idle;
    [SerializeField] AnimationClip phase1Attack;
    [SerializeField] AnimationClip phase1Hurt;
    [SerializeField] AnimationClip phase2Idle;
    [SerializeField] AnimationClip phase2Attack;
    [SerializeField] AnimationClip phase2Hurt;
    [SerializeField] AnimationClip rebirth;
    [SerializeField] AnimationClip death;
    Animator animator;
    
    [Header("Attack Info")]
    [SerializeField] float timeBetweenP1Attacks;
    [SerializeField] float timeBetweenP2Attacks;
    [SerializeField] float idleTimeP1;
    [SerializeField] float idleTimeP2;
    [SerializeField] float slashSpeedP1;
    [SerializeField] float slashSpeedP2;
    [SerializeField] int damageP1;
    [SerializeField] int damageP2;
    private float timeSinceLastAttack;
    private float idleTime;
    private int numAttacks;
    private string state;

    private float animTime;
    private bool phase1 = true;
    public float hp = 40;
    private float phase2HpThreshold = 30;
    private float damageCooldown;

    List<Transform> topTransforms;
    List<Transform> leftTransforms;
    List<Transform> rightTransforms;
    List<Transform> roundTopTransforms;
    List<Transform> roundBottomTransforms;

    Dictionary<int, List<Transform>> listMappings;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] GameObject fireballPrefab;


    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerMovement = player.GetComponent<PlayerMovement>();
        entityMovement = player.GetComponent<Movement>();
        animator = GetComponent<Animator>();
        hazardDamager = GetComponent<HazardDamager>();

        state = "beforeBattle";

        populateTransformArrays();
    }

    // Update is called once per frame
    void Update()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        doDamageCooldown();

        if (hp <= phase2HpThreshold && phase1)
        {
            phase1 = false;
            setAnimation(rebirth.name, true);
            state = "phase2Start";
        }

        stateMachine();
    }

    void populateTransformArrays()
    {
        topTransforms = new List<Transform>();
        rightTransforms = new List<Transform>();
        leftTransforms = new List<Transform>();
        roundTopTransforms = new List<Transform>();
        roundBottomTransforms = new List<Transform>();

        foreach(Transform topTransform in transform)
        {
            foreach(Transform childTransform in topTransform)
            {
                childTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                if (topTransform.name == "Top")
                {
                    topTransforms.Add(childTransform);
                } else if (topTransform.name == "Left")
                {
                    leftTransforms.Add(childTransform);
                } else if (topTransform.name == "Right")
                {
                    rightTransforms.Add(childTransform);
                } else if (topTransform.name == "RoundTop")
                {
                    roundTopTransforms.Add(childTransform);
                } else if (topTransform.name == "RoundBottom")
                {
                    roundBottomTransforms.Add(childTransform);
                }
            }
        }

        listMappings = new Dictionary<int, List<Transform>>
        {
            {0, topTransforms },
            {1, rightTransforms},
            {2, leftTransforms},
            {3, roundTopTransforms},
            {4, roundBottomTransforms}
        };

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

    private Quaternion getRotation(int attackDir, Transform t)
    {
        if (attackDir == 0)
        {
            return Quaternion.identity;
        } else if (attackDir == 1)
        {
            return Quaternion.Euler(0, 0, -90);
        } else if (attackDir == 2)
        {
            return Quaternion.Euler(0, 0, 90);
        }

        return Quaternion.identity;
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) || animator.GetCurrentAnimatorStateInfo(0).IsName(rebirth.name) || damageCooldown != 0 || hazardDamager.doesDamage)
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
            if (phase1)
            {
                damageCooldown = phase1Hurt.length*0.75f;
                setAnimation(phase1Hurt.name, true);   
            } else
            {
                damageCooldown = phase2Hurt.length*0.6f;
                setAnimation(phase2Hurt.name, true);
            }
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
        } else if (state == "blowback")
        {
            auraFarm();
            numAttacks = Random.Range(2, 5);
        } else if (state == "afterAuraFarm")
        {
            afterAuraFarm();
        } else if (state == "attack1")
        {
            phase1AttackFunc();
        } else if (state == "waitPhase1Attack")
        {
            phase1AttackWaitFunc();
        } else if (state == "phase1Idle")
        {
            phase1IdleFunc();
        } else if (state == "phase2Start")
        {
            phase2StartFunc();
        } else if (state == "attack2")
        {
            phase2AttackFunc();
        } else if (state == "waitPhase2Attack")
        {
            phase2AttackWaitFunc();
        } else if (state == "phase2Idle")
        {
            phase2IdleFunc();
        } else if (state == "dead")
        {
            if (animTime > 1 && animator.GetCurrentAnimatorStateInfo(0).IsName(death.name))
            {
                gm.phoenixDefeated = true;
                gm.sceneTransition = true;
                Destroy(gameObject);
            } else if (!animator.GetCurrentAnimatorStateInfo(0).IsName(death.name))
            {
                setAnimation(death.name, true);
            }
        }
    }

    private void beforeBattleFunc()
    {
        float dist = getPlayerDist();
        if (dist != -1 && dist < 15)
        {
            // phoenixBlock.GetComponent<BoxCollider2D>().enabled = true;
            // playerCamera.GetComponent<PlayerCamera>().changeSize = true;
            setAnimation(phase1Attack.name, true);
            state = "blowback";
        } else
        {
            setAnimation(phase1Idle.name, false);
        }
    }

    private void auraFarm()
    {
        if (animTime > 0.5)
        {
            entityMovement.velocity = new Vector2(0, -0.1f); //*  entityMovement.velocity.normalized;
            state = "afterAuraFarm";
        }
    }

    private void afterAuraFarm()
    {
        if (idleTime > 1f && phase1)
        {
            hazardDamageModifier(true);
            idleTime = 0;
            state = "attack1";
            return;
        } else if (idleTime > 1f && !phase1)
        {
            hazardDamageModifier(true);
            idleTime = 0;
            state = "attack2";
            return;
        }

        idleTime += Time.deltaTime;
    }

    private void phase1AttackFunc()
    {
        int attackDir = Random.Range(0, 3);
        setAnimation(phase1Attack.name, true);

        foreach(Transform t in listMappings[attackDir])
        {
            Quaternion rot = getRotation(attackDir, t);
            GameObject g = Instantiate(slashPrefab, t.position, rot);
            PhoenixSlash ps = g.GetComponent<PhoenixSlash>();
            ps.setDirection(rot * Vector3.down);
            ps.setSpeed(slashSpeedP1);
            ps.setWaitTime(1f);
            ps.setDamage(damageP1);
        }
        state = "waitPhase1Attack";
        numAttacks -= 1;
        timeSinceLastAttack = 0;
    }

    private void phase1AttackWaitFunc()
    {
        if (numAttacks == 0)
        {
            state = "phase1Idle";
            return;
        }
        if (timeSinceLastAttack > timeBetweenP1Attacks)
        {
            state = "attack1";
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    private void phase1IdleFunc()
    {
        hazardDamageModifier(false);
        setAnimation(phase1Idle.name, false);
        if (idleTime > idleTimeP1)
        {
            idleTime = 0;
            state = "blowback";
            setAnimation(phase1Attack.name, true);
            hazardDamageModifier(true);
            return;
        }

        idleTime += Time.deltaTime;
    }

    private void phase2StartFunc()
    {
        Debug.Log(animTime);
        if (animTime >= 1)
        {
            state = "blowback";
        }
    }

    private void phase2AttackFunc()
    {
        int attackType;
        if (numAttacks == 1)
        {
            attackType = 1;
        } else
        {
            attackType = Random.Range(0, 2);    
        }

        if (attackType == 0)
        {
            doSlashAttack();
        } else
        {
            doFireballAttack();
        }
        
        state = "waitPhase2Attack";
        numAttacks -= 1;
        timeSinceLastAttack = 0;
    }

    private void doSlashAttack()
    {
        int attackDir = Random.Range(0, 3);
        setAnimation(phase2Attack.name, true);
        foreach(Transform t in listMappings[attackDir])
        {
            Quaternion rot = getRotation(attackDir, t);
            GameObject g = Instantiate(slashPrefab, t.position, rot);
            PhoenixSlash ps = g.GetComponent<PhoenixSlash>();
            ps.setDirection(rot * Vector3.down);
            ps.setSpeed(slashSpeedP2);
            ps.setWaitTime(1f);
            ps.setDamage(damageP2);
        }
    }

    private void doFireballAttack()
    {
        float midTop = roundTopTransforms[0].transform.position.y;
        float midBot = roundBottomTransforms[0].transform.position.y;

        int attackDir;
        float playerY = player.transform.position.y;
        if (playerY < midTop && playerY > midBot)
        {
            attackDir = Random.Range(3, 5);
        } else if (playerY < midTop)
        {
            attackDir = 3;
        } else
        {
            attackDir = 4;
        }

        foreach(Transform t in listMappings[attackDir])
        {
            GameObject g = Instantiate(fireballPrefab, t.position, t.rotation);
            PhoenixFireball ps = g.GetComponent<PhoenixFireball>();
            ps.setDirection(t.rotation * Vector3.down);
            ps.setSpeed(slashSpeedP2);
            ps.setWaitTime(1f);
            ps.setDamage(damageP2);
        }
    }

    private void phase2AttackWaitFunc()
    {
        if (numAttacks == 0)
        {
            state = "phase2Idle";
            return;
        }
        if (timeSinceLastAttack > timeBetweenP1Attacks)
        {
            state = "attack2";
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    private void phase2IdleFunc()
    {
        hazardDamageModifier(false);
        setAnimation(phase2Idle.name, false);
        if (idleTime > idleTimeP2)
        {
            idleTime = 0;
            state = "blowback";
            setAnimation(phase2Attack.name, true);
            hazardDamageModifier(true);
            return;
        }

        idleTime += Time.deltaTime;
    }

    private float getPlayerDist()
    {
        if (player == null)
        {
            return -1;
        }
        Vector2 myPos = new Vector2(transform.position.x-2f, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(myPos, playerMovement.truePos - myPos);
        Debug.DrawRay(myPos, playerMovement.truePos - myPos);

        if (hit && hit.transform.gameObject.name == "Player")
        {
            return hit.distance;
        }

        return -1;
    }
}
