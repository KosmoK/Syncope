using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] Transform player;
    NavMeshAgent agent;
    public float hitDist = 10;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;        
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, hitDist);

        if (hit && hit.transform == player.transform)
        {
            agent.SetDestination(player.position);
        }
    }
}
