using UnityEngine;

public class EnemyMovementState : MonoBehaviour
{
    [SerializeField] string enemyType;

    private string state;
    [SerializeField] float idleToChaseDist;
    [SerializeField] float chaseFollowDist;
    [SerializeField] float attackDist;
    EnemyAnimStates animStates;
    PlayerMovement player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = "idle";
        animStates = GetComponent<EnemyAnimStates>();
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        string newState = checkCurrentState();
        if (animStates.canChangeState())
        {
            // Debug.Log($"{gameObject.name} {GetInstanceID()} state {state} -> {newState}");
            state = newState;
        }
    }

    public string getState()
    {
        return state;
    }
    public string getEnemyType()
    {
        return enemyType;
    }

    public PlayerMovement getPlayer()
    {
        return player;
    }

    private string checkCurrentState()
    {
        float dist = getPlayerDist();
        if (dist == -1)
        {
            return "idle";
        }
        if ((state == "attack" || state == "chase") && dist < attackDist && animStates.getAttackCooldown() == 0)
        {
            return "attack";
        }
        if (state == "idle" && dist < idleToChaseDist)
        {
            return "chase";
        }
        if ((state == "attack" || state == "chase") && dist < chaseFollowDist)
        {
            return "chase";
        }
        
        return "idle";
    }

    private float getPlayerDist()
    {
        if (player == null)
        {
            return -1;
        }
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.truePos - myPos);
        Debug.DrawRay(transform.position, player.truePos - myPos);

        if (hit && hit.transform.gameObject.name == "Player")
        {
            return hit.distance;
        }

        return -1;
    }


}
