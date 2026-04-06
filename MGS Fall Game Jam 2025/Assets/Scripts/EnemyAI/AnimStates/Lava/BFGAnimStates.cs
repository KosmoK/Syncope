using UnityEngine;

public class BFGAnimStates : AnimStatesBase
{
    [SerializeField] AnimationClip idleAwake;
    protected override void animHandler()
    {
        // Debug.Log($"bfg state: {state}");
        if (state == "idle")
        {
            bfgIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            bfgWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            bfgChase();
        } else if (state == "attack")
        {
            bfgAttack();
        }
    }

    private void bfgIdle()
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
    private void bfgWake()
    {
        setAnimation(alert.name, true);
    }
    private void bfgChase()
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
    private void bfgAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
        }
    }
}
