using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private EntityStatus playerStatus;
    private PlayerMovement playerMovement;
    private InputAction attackAction;
    [SerializeField] AnimationClip attack1;
    [SerializeField] AnimationClip attack2;
    [SerializeField] AnimationClip dash;
    // [SerializeField] GameObject attackColliderObject;
    // private BoxCollider2D attackCollider;
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        animator = GetComponent<Animator>();
        playerStatus = GetComponent<EntityStatus>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private bool canDoDamage = false;
    public bool queueFirstAttack = false;
    public bool queueSecondAttack = false;
    private float animTime;

    public int bonusLavaDamage;
    public int bonusIceDamage;

    
    void Update()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        bool success;
        if (queueSecondAttack && animTime > 0.9)
        {
            success = setAnimation(attack2.name, true);
            if (success)
            {
                queueSecondAttack = false;
                canDoDamage = true;
            }
        } else if (queueFirstAttack)
        {
            success = setAnimation(attack1.name, true);
            if (success)
            {
                queueFirstAttack = false;
                canDoDamage = true;
            }
        } 
    }

    void FixedUpdate()
    {
        if (playerStatus.IsDead())
        {
            return;
        }
        if (attackAction.WasPressedThisFrame() && !animator.GetCurrentAnimatorStateInfo(0).IsName(dash.name))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(attack1.name)) //&& animTime > 0.5
            {
                queueSecondAttack = true;
            } else if (!animator.GetCurrentAnimatorStateInfo(0).IsName(attack1.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(attack2.name))
            {
                queueFirstAttack = true;
            }
        }
    }

    public bool isAttacking()
    {
        if (queueSecondAttack || queueFirstAttack)
        {
            return true;
        }
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName(attack1.name) || animator.GetCurrentAnimatorStateInfo(0).IsName(attack2.name)) && animTime < 0.95)
        {
            return true;
        }

        return false;
    }

    private bool setAnimation(string name, bool ignoreTime)
    {
        if (!ignoreTime && animTime < 1)
        {
            return false;
        }

        animator.Play(name, -1, 0f);

        return true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        colliderLogic(collider);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        colliderLogic(collider);
    }

    private void colliderLogic(Collider2D collider)
    {
        // Debug.Log(collider.name);
        bool atk1 = animator.GetCurrentAnimatorStateInfo(0).IsName(attack1.name);
        bool atk2 = animator.GetCurrentAnimatorStateInfo(0).IsName(attack2.name);

        if (!(atk1 || atk2) || !withinAnimBounds(atk1, atk2))
        {
            return;
        }

        string tag = collider.gameObject.tag;
        if (!(tag == "LavaEnemy" || tag == "IceEnemy" || tag == "DeflectAttack" || tag == "Phoenix" || tag == "Golem"))
        {
            return;
        }

        if (!canDoDamage)
        {
            return;
        }

        if (tag == "LavaEnemy")
        {
            if (atk1)
            {
                collider.gameObject.GetComponent<EnemyAnimStates>().dealDamage(1+bonusLavaDamage);
            } else if (atk2)
            {
                collider.gameObject.GetComponent<EnemyAnimStates>().dealDamage(2+bonusLavaDamage);
            }

            // for kb, but not gonna use. If you do use then make sure to add rigid bodies
            // Vector2 force = playerMovement.GetLastKnownDirection().normalized;
            // collider.attachedRigidbody.AddForce(force*forceMag);
            // Debug.Log("applied force?");
        } else if (tag == "IceEnemy")
        {
              if (atk1)
            {
                collider.gameObject.GetComponent<EnemyAnimStates>().dealDamage(1+bonusIceDamage);
            } else if (atk2)
            {
                collider.gameObject.GetComponent<EnemyAnimStates>().dealDamage(2+bonusIceDamage);
            }
        } else if (tag == "Phoenix")
        {
            if (atk1)
            {
                collider.gameObject.GetComponent<Phoenix>().dealDamage(1+bonusLavaDamage);
            } else if (atk2)
            {
                collider.gameObject.GetComponent<Phoenix>().dealDamage(2+bonusLavaDamage);
            }
        } else if (tag == "Golem")
        {
            if (atk1)
            {
                collider.gameObject.GetComponent<Golem>().dealDamage(1+bonusIceDamage);
            } else if (atk2)
            {
                collider.gameObject.GetComponent<Golem>().dealDamage(2+bonusIceDamage);
            }
        }

        canDoDamage = false;
    }

    private bool withinAnimBounds(bool atk1, bool atk2)
    {
        if (atk1)
        {
            return animTime > 0.2 && animTime < 0.8;
        } if (atk2)
        {
            return animTime > 0.2 && animTime < 0.9;
        }

        return false;
    }

    
}
