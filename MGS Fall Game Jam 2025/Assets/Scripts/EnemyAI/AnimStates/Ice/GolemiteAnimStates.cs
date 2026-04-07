using UnityEngine;
using UnityEngine.AI;

public class Golemite : AnimStatesBase
{
    Vector3 runToPoint;
    bool runningToPoint;
    protected override void animHandler()
    {

        if (state == "idle" && !runningToPoint)
        {
            golemiteIdle();
        } else if (state == "chase" && prevState == "idle")
        {
            golemiteAlert();
        } else if (state == "chase" && !(currentAnim == alert.name && animTime < 1))
        {
            golemiteRun();
        } else if (state == "attack")
        {
            // golemiteAttack();
        }

        if (runningToPoint)
        {
            Debug.Log($"Golemite running dist {(runToPoint-transform.position).magnitude}");
        }

        if (runningToPoint && (runToPoint-transform.position).magnitude < 4f)
        {
            runningToPoint = false;
            golemiteAttack();
        }
    }

    private void golemiteIdle()
    {
        if (agent.enabled && lastKnownDir != Vector2.zero)
        {
            agent.enabled = false;
        }

        setAnimation(idle.name, false);    
        
    }
    private void golemiteAlert()
    {
        setAnimation(alert.name, true);
    }
    private void golemiteRun()
    {
        if (!agent.enabled && !sketchTornado)
        {
            agent.enabled = true;
        }
        bool success = setAnimation(move.name, false);
        flipRenderer();

        if (success && !runningToPoint)
        {
            runToPoint = findPointToRunTo();
            agent.SetDestination(runToPoint);
            runningToPoint = true;
            Debug.Log($"Golemite running to point {runToPoint}");
        }
    }
    private void golemiteAttack()
    {
        bool success = setAnimation(attack.name, true);
        if (success)
        {
            createAttack();
            playAttackSound(FMODEvents.instance.antLeaderAttack);
        }
    }

    private Vector3 findPointToRunTo()
    {
        Vector3 point;
        Vector3 direction = (transform.position - player.transform.position).normalized;
        float dist = 10;
        float boxSize = 2;
        do
        {
            point = transform.position + direction * dist;
            point.x += ((Random.value > 0.5) ? 1 : -1) * Random.value * boxSize;
            point.y += ((Random.value > 0.5) ? 1 : -1) * Random.value * boxSize;

            dist -= 0.5f;
            boxSize += 0.5f;

        } while (!IsPointOnNavMesh(point));

        return point;
    }
    private bool IsPointOnNavMesh(Vector3 point, float maxDistance = 1.0f)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(point, out hit, maxDistance, GetComponent<NavMeshAgent>().areaMask);
    }
}
