using UnityEngine;

public class RattlebonesAnimStates : AnimStatesBase
{
    [SerializeField] AnimationClip idleAwake;
    [SerializeField] AnimationClip slitherStart;
    protected override void animHandler()
    {
        if (state == "idle")
        {
            snakeIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            snakeWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            snakeChase();
        } else if (state == "attack")
        {
            snakeAttack();
        }
    }

        private void snakeIdle()
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
    private void snakeWake()
    {
        setAnimation(alert.name, true);
    }
    private void snakeChase()
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }
        
        bool success;
        if (currentAnim == alert.name)
        {
            success = setAnimation(slitherStart.name, false);
        } else
        {
            success = setAnimation(move.name, false);   
        }
        flipRenderer();

        if (success)
        {
            agent.SetDestination(player.truePos);   
        }
    }
    private void snakeAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
            playAttackSound(FMODEvents.instance.snakeAttack);
        }
    }
}
