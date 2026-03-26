using UnityEngine;

public class EmberAntAnimStates : AnimStatesBase
{
    // regular ant stuff
    protected override void animHandler()
    {
        if (state == "idle")
        {
            antIdle();
        } else if (state == "chase")
        {
            antChase();
        } else if (state == "attack")
        {
            antAttack();
        }
    }

    private void antIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }
        if (prevState != "idle")
        {
            idleWaitTime = 0;
        }

        setAnimation(idle.name, false);    

        idleWaitTime += Time.deltaTime;
        
    }
    private void antChase()
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
    private void antAttack()
    {
        bool success = setAnimation(attack.name, false, "AttackSfx");
        if (success)
        {
            createAttack();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.antAttack, this.transform.position);
        }
    }
}
