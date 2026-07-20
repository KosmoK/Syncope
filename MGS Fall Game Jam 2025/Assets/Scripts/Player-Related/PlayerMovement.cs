using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using System.Collections.Generic;
using FMOD.Studio;

public class PlayerMovement : MonoBehaviour
{
    // Movement System class
    private Movement movement; // Requires the Movement script in the same object as the PlayerMovement script
    private PlayerAttack playerAttack;

    // Movement Input Keys
    private InputAction moveAction; // Input System for Movement
    private InputAction attackAction; // Input System for Attacking
    private InputAction interactAction; // Input System for Interaction
    private InputAction extraAttackAction;
    private InputAction dash;
    private InputAction lavaBB;
    private InputAction iceBB;

    // Adjust Player settings
    [SerializeField] public AnimationCurve dashCurve = AnimationCurve.Constant(0f, 1f, 1f);
    private float dashStrength = 1.0f;
    public float dashStrengthMultiplier = 1.0f;
    public float dashCooldown = 0.75f;
    public float dashTime = 0.5f;
    private Vector2 lastKnownDirection = new(1f,0f);

    // SFX
    private bool hasMovedThisFrame;
    private bool playedAnimationThisFrame = true;
    private Animator animator; // Requires Animator component to be in the same object as PlayerMovement script
    private EntityStatus playerStatus;
    private bool isHurt = false;
    private bool isDashing = false;
    private bool isAttacking = false;
    public Vector2 truePos;
    [SerializeField] AnimationClip sideIdlePlayer;
    [SerializeField] AnimationClip sideMovePlayer;
    [SerializeField] AnimationClip frontIdlePlayer;
    [SerializeField] AnimationClip frontMovePlayer;
    [SerializeField] AnimationClip backIdlePlayer;
    [SerializeField] AnimationClip backMovePlayer;
    [SerializeField] AnimationClip dashAnim;
    [SerializeField] AnimationClip attack1Anim;
    [SerializeField] AnimationClip attack2Anim;
    [SerializeField] GameObject attack1Prefab;
    [SerializeField] GameObject attack2Prefab;
    [SerializeField] List<AnimationClip> extraAttacks;
    [SerializeField] int currExtra = -1;
    private bool queueFirstAttack;
    private bool queueSecondAttack;
    private bool queueExtraAttack;
    [SerializeField] List<GameObject> attackPrefabs;
    private GameObject attackObject;

    private EventInstance playerFootsteps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set InputAction Variables
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        extraAttackAction = InputSystem.actions.FindAction("Extra Attack");
        interactAction = InputSystem.actions.FindAction("Interact");
        dash = InputSystem.actions.FindAction("Dash");
        lavaBB = InputSystem.actions.FindAction("Fire Boss Button");
        iceBB = InputSystem.actions.FindAction("Ice Boss Button");

        // Dash Coroutine
        StartCoroutine(DashCoroutine());

        // Gettitng scripts
        movement = GetComponent<Movement>();
        playerStatus = GetComponent<EntityStatus>();

        // Getting Animation
        animator = transform.GetChild(0).GetComponent<Animator>();

        playerAttack = GetComponent<PlayerAttack>();
        playerAttack.playerMovement = this;

        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);
    }

    // Update is called once per fixed update
    void FixedUpdate()
    {
        truePos = new Vector2(transform.position.x, transform.position.y+0.1f);
        
        playedAnimationThisFrame = false;
        isHurt = playerStatus.IsStunned();
        isAttacking = isAttackingFunc();
        isDashing = (animator.GetCurrentAnimatorStateInfo(0).IsName(dashAnim.name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95) ? true : false;

        // Movement
        if (!isHurt || playerStatus.IsDead())
        {
            hasMovedThisFrame = moveAction.ReadValue<Vector2>() != Vector2.zero;
            movement.moveDirection = moveAction.ReadValue<Vector2>();   
        } else
        {
            movement.moveDirection = Vector2.zero;
        }

        if (hasMovedThisFrame) 
        {
            lastKnownDirection = movement.moveDirection;
        }
        if (playerStatus.IsDead())
        {
            enabled = false;
        }

        // Update idle sprite to Correct Values
        if (!playerStatus.IsDead())
        {
            setAnimation(Vector2.up, Math.Sqrt(2)/2, backIdlePlayer, backMovePlayer);
            setAnimation(Vector2.right, Math.Sqrt(2)/2, sideIdlePlayer, sideMovePlayer);
            setAnimation(Vector2.left, Math.Sqrt(2)/2, sideIdlePlayer, sideMovePlayer);
            setAnimation(Vector2.down, Math.Sqrt(2)/2, frontIdlePlayer, frontMovePlayer);
        }

        attackLogic();

        UpdateSound();
    }

    void Update()
    {
        if (iceBB.WasPressedThisFrame())
        {
            transform.position = new Vector3(37.84f, -19.01f, 0);
        }
        if (lavaBB.WasPressedThisFrame())
        {
            transform.position = new Vector3(-1149.4f, -14.2f, 0);
        }


        if (attackAction.WasPressedThisFrame() && !animator.GetCurrentAnimatorStateInfo(0).IsName(dash.name))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(attack1Anim.name)) //&& animTime > 0.5
            {
                queueSecondAttack = true;
            } else if (!animator.GetCurrentAnimatorStateInfo(0).IsName(attack1Anim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(attack2Anim.name))
            {
                queueFirstAttack = true;
            }
        }

        if (currExtra >= 0 && extraAttackAction.WasPressedThisFrame() && !animator.GetCurrentAnimatorStateInfo(0).IsName(dash.name) && GetComponent<SpecialCooldown>() == null)
        {
            queueExtraAttack = true;
        }
    }

    private void attackLogic()
    {
        if (isDashing && attackObject != null)
        {
            Destroy(attackObject);
        }

        float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (queueSecondAttack && animTime > 0.8)
        {
            queueSecondAttack = false;
            attackObject = Instantiate(attack2Prefab, transform);
            animator.Play(attack2Anim.name, -1, 0f);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerAttack, this.transform.position);
            // StartCoroutine(setAttacking(attack2Anim));
        } else if (queueFirstAttack)
        {
            queueFirstAttack = false;
            attackObject = Instantiate(attack1Prefab, transform);
            animator.Play(attack1Anim.name, -1, 0f);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerAttack, this.transform.position);
            // StartCoroutine(setAttacking(attack1Anim));
        }

        if (queueExtraAttack)
        {
            queueExtraAttack = false;
            attackObject = Instantiate(attackPrefabs[currExtra], transform);
            animator.Play(extraAttacks[currExtra].name, -1, 0f);
        }
    }

    // IEnumerator setAttacking(AnimationClip clip)
    // {
    //     isAttacking = true;
    //     yield return new WaitForSeconds(clip.length);
    //     Debug.Log($"finished attacking, norm time: {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}, len: {clip.length}, {animator.GetCurrentAnimatorClipInfoCount(0)}, {animator.GetCurrentAnimatorClipInfo(0)}");
    //     isAttacking = false;
    // }

    private bool isAttackingFunc()
    {
        float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (queueSecondAttack || queueFirstAttack)
        {
            return true;
        }
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName(attack1Anim.name) || animator.GetCurrentAnimatorStateInfo(0).IsName(attack2Anim.name) || 
            ((currExtra != -1) ? animator.GetCurrentAnimatorStateInfo(0).IsName(extraAttacks[currExtra].name) : false)) && animTime < 0.95)
        {
            return true;
        }

        return false;
    }

    IEnumerator DashCoroutine()
    {
        float dashTimer = 0f;
        dashStrength = dashCurve.Evaluate(0f);

        while(true){
            // Dashing
            if (dash.WasPressedThisFrame() && !playerStatus.IsDead())
            {
                movement.velocity = lastKnownDirection * dashStrength;
                
                if (Vector2.Dot(lastKnownDirection, new(1f,0f)) < 0 && !isHurt)
                {
                    transform.localScale = new(-1, transform.localScale.y);
                }
                if (Vector2.Dot(lastKnownDirection, new(-1f,0f)) < 0 && !isHurt)
                {
                    transform.localScale = new(1, transform.localScale.y);
                }

                animator.Play(dashAnim.name);
                playedAnimationThisFrame = true;

                for(dashTimer = 0f; dashTimer < dashTime; dashTimer += Time.fixedDeltaTime)
                {
                    dashStrength = dashCurve.Evaluate(dashTimer/dashTime);
                    movement.velocity = (lastKnownDirection * dashStrength * dashStrengthMultiplier) * Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                dashTimer = 0f;
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void setAnimation(Vector2 compareVector, double compareDot, AnimationClip setIdleSprite, AnimationClip setMoveSprite)
    { 
        // Function to set the  sprite
        // Gets the DOT product of the inputted vector to the last known direction
        // If the dot product is within compareDot and you havent moved this frame, you set the idle sprite to the value you set.
        // If you did move, then you apply the correct movement sprite
        bool isWithin = Vector2.Dot(lastKnownDirection, compareVector) >= compareDot;
        
        if (isWithin && !hasMovedThisFrame && !playedAnimationThisFrame && !isHurt && !playerStatus.IsDead() && !isDashing && !isAttacking)
        {
            animator.Play(setIdleSprite.name);
            playedAnimationThisFrame = true;
        }
        else if(isWithin && hasMovedThisFrame && !playedAnimationThisFrame && !isHurt && !playerStatus.IsDead() && !isDashing && !isAttacking)
        {
            animator.Play(setMoveSprite.name);
            playedAnimationThisFrame = true;
        }

        // Flip the sprite in the Xdirection if the player's XmoveDirection is the opposite
        if (Vector2.Dot(lastKnownDirection, new(1f,0f)) < 0 && !isHurt)
        {
            transform.localScale = new(-1, transform.localScale.y);
        }
        if(Vector2.Dot(lastKnownDirection, new(-1f,0f)) < 0 && !isHurt)
        {
            transform.localScale = new(1, transform.localScale.y);
        }
    }

    public Vector2 GetLastKnownDirection()
    {
        return lastKnownDirection;
    }

    public void setCurrExtra(int extra)
    {
        currExtra = extra;
    }
    public bool HasMovedThisFrame(){return hasMovedThisFrame;}

    private void UpdateSound() {
        if (hasMovedThisFrame)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
