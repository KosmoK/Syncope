using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    // Movement System class
    private Movement movement; // Requires the Movement script in the same object as the PlayerMovement script
    private PlayerAttack playerAttack;

    // Movement Input Keys
    private InputAction moveAction; // Input System for Movement
    private InputAction attackAction; // Input System for Attacking
    private InputAction interactAction; // Input System for Interaction
    private InputAction dash;

    // Adjust Player settings
    public float dashStrength = 1.0f;
    public float dashCooldown = 0.75f;
    private Vector2 lastKnownDirection = new(1f,0f);

    // SFX
    private bool hasMovedThisFrame;
    private bool playedAnimationThisFrame = true;
    private Animator animator; // Requires Animator component to be in the same object as PlayerMovement script
    private EntityStatus playerStatus;
    private bool isHurt = false;
    private bool isDashing = false;
    private bool isAttacking = false;
    public AnimationClip sideIdlePlayer;
    public AnimationClip sideMovePlayer;
    public AnimationClip frontIdlePlayer;
    public AnimationClip frontMovePlayer;
    public AnimationClip backIdlePlayer;
    public AnimationClip backMovePlayer;
    public AnimationClip dashAnim;
    public Vector2 truePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set InputAction Variables
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        interactAction = InputSystem.actions.FindAction("Interact");
        dash = InputSystem.actions.FindAction("Dash");

        // Dash Coroutine
        StartCoroutine(DashCoroutine());

        // Gettitng scripts
        movement = GetComponent<Movement>();
        playerStatus = GetComponent<EntityStatus>();

        // Getting Animation
        animator = GetComponent<Animator>();

        playerAttack = GetComponent<PlayerAttack>();
    }

    // Update is called once per fixed update
    void FixedUpdate()
    {
        truePos = new Vector2(transform.position.x, transform.position.y+0.3f);
        playedAnimationThisFrame = false;
        isHurt = playerStatus.IsStunned();
        isAttacking = playerAttack.isAttacking();
        isDashing = (animator.GetCurrentAnimatorStateInfo(0).IsName(dashAnim.name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95) ? true : false;

        // Movement
        if (!isHurt || playerStatus.IsDead())
        {
            hasMovedThisFrame = moveAction.ReadValue<Vector2>() != Vector2.zero;
            movement.moveDirection = moveAction.ReadValue<Vector2>();   
        }
        else{movement.moveDirection = Vector2.zero;}

        if(hasMovedThisFrame) {lastKnownDirection = movement.moveDirection;}
        if(playerStatus.IsDead()){this.enabled = false;}

        // Update idle sprite to Correct Values
        if (!playerStatus.IsDead())
        {
            setAnimation(new(0f, 1f), Math.Sqrt(2)/2, backIdlePlayer, backMovePlayer);
            setAnimation(new(1f, 0f), Math.Sqrt(2)/2, sideIdlePlayer, sideMovePlayer);
            setAnimation(new(-1f, 0f), Math.Sqrt(2)/2, sideIdlePlayer, sideMovePlayer);
            setAnimation(new(0f, -1f), Math.Sqrt(2)/2, frontIdlePlayer, frontMovePlayer);
        }
    }

    IEnumerator DashCoroutine()
    {
        while(true){
            // Dashing
            if (dash.WasPressedThisFrame() && !playerStatus.IsDead())
            {
                movement.velocity += lastKnownDirection * dashStrength;
                
                if(Vector2.Dot(lastKnownDirection, new(1f,0f)) < 0 && !isHurt){transform.localScale = new(-1, transform.localScale.y);}
                if(Vector2.Dot(lastKnownDirection, new(-1f,0f)) < 0 && !isHurt){transform.localScale = new(1, transform.localScale.y);}
                animator.Play(dashAnim.name);
                playedAnimationThisFrame = true;

                yield return new WaitForSeconds(dashCooldown);
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

    public Vector2 GetLastKnownDirection(){return lastKnownDirection;}
}
