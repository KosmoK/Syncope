using UnityEngine;
using UnityEngine.AI;

public class SharmadilloAnimStates : AnimStatesBase
{
    [SerializeField] AnimationClip idleAwake;
    protected override void animHandler()
    {
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
            Debug.Log($"Sharm speed: {GetComponent<NavMeshAgent>().speed}");
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
            setAnimation(idleAwake.name, false);
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

        // if (success)
        // {
            agent.SetDestination(player.truePos);
        // }
    }
    private void sharmadilloAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
            agent.SetDestination(player.truePos);
        }
    }
}
