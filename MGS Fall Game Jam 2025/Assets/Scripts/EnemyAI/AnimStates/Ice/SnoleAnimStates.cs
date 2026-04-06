using System.Collections;
using UnityEngine;

public class SnoleAnimStates : AnimStatesBase
{
    SpriteRenderer spriteRendererRef;
    Transform playerTransformRef;

    new void Start()
    {
        base.Start();

        spriteRendererRef = GetComponent<SpriteRenderer>(); 
        playerTransformRef = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void animHandler()
    {
        if (state == "idle")
        {
            snoleIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            snoleWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            snoleIdle();
        } else if (state == "attack")
        {
            snoleAttack();
        }

        if (playerTransformRef.position.x > transform.position.x)
        {
            spriteRendererRef.flipX = true;
        }
        else 
        {
            spriteRendererRef.flipX = false;
        }
    }

    private void snoleIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }
        
        setAnimation(idle.name, false); 
        
    }
    private void snoleWake()
    {
        setAnimation(alert.name, true);
    }
    
    private void snoleAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            StartCoroutine(spawnAttack(0.9f)); // hard coded number that roughly corresponds with the animation
        }
    }

    IEnumerator spawnAttack(float length)
    {
        yield return new WaitForSeconds(length);
        createAttack();
        playAttackSound(FMODEvents.instance.antLeaderAttack);
    }
}
