using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EntityStatus : MonoBehaviour
{
    GameManager gm;
    public int health = 10;
    [SerializeField] private bool iFramesActive = false;
    public bool isStunned;
    public float iFramesLength = 1f; 
    public bool isDead = false;
    [SerializeField] private float hurtAnimTime = 0.5f;
    public List<StatusEffects> statusEffects;
    private Animator animator;
    private Movement entityMovement;
    private PlayerMovement pm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // IFrames Coroutine
        StartCoroutine(IFramesCoroutine());
        animator = GetComponent<Animator>();
        entityMovement = GetComponent<Movement>();
        pm = GetComponent<PlayerMovement>();
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>();
    }

    IEnumerator IFramesCoroutine()
    {
        while (!isDead)
        {
            if (iFramesActive)
            {
                isStunned = true;
                yield return new WaitForSeconds(hurtAnimTime);
                isStunned = false;
                yield return new WaitForSeconds(iFramesLength);
                iFramesActive = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    public void Hurt(int damage)
    { // Use this to hurt the player
        if (!iFramesActive && !isDead && !isDead) 
        {
            iFramesActive = true; 
            TrueHurt(damage);
        }
    }
    public void TrueHurt(int damage)
    { // Use this to hurt the player + ignore iframmes
        if(damage >= health) {health = 0; KillEntity();}
        else {health = health - damage;}
        
        if(!isDead){animator.Play("Hurt"); Debug.Log("I got hurt!");}
    }
    public void KillEntity()
    {
        health = 0;
        isDead = true;
        animator.Play("Death");
        Debug.Log("Playing death anim");
        entityMovement.moveDirection = Vector2.zero;
        entityMovement.velocity = Vector2.zero;

        if (pm != null) {pm.enabled = false;}
        StartCoroutine(DeathCoroutine());
    }
    public void ApplyStatus(StatusEffects effect) {statusEffects.Add(effect);}
    public bool IsIFramesActive(){return iFramesActive;}
    public bool IsStunned(){return isStunned;}
    public bool IsDead(){return isDead;}
}
