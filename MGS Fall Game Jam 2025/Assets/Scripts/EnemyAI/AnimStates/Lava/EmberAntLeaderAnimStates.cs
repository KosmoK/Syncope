using UnityEngine;

public class EmberAntLeaderAnimStates : AnimStatesBase
{
    [SerializeField] AnimationClip idleAwake;
    protected override void animHandler()
    {
        animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (dead)
        {
            if (agent.enabled == true)
            {
                agent.enabled = false;   
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(death.name) && animTime >= 1)
            {
                Destroy(gameObject);
            } else
            {
                return;
            }
        }

        if (state == "idle")
        {
            antLeaderIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            antLeaderWake();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            antLeaderChase();
        } else if (state == "attack")
        {
            antLeaderAttack();
        }
    }

    private void antLeaderIdle()
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
    private void antLeaderWake()
    {
        setAnimation(alert.name, true);
    }
    private void antLeaderChase()
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
    private void antLeaderAttack()
    {
        bool success = setAnimation(attack.name, false);
        if (success)
        {
            createAttack();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.antLeaderAttack, this.transform.position);
        }
    }
}
