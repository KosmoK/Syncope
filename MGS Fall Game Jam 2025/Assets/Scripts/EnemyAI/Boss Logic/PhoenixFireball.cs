using UnityEngine;

public class PhoenixFireball : MonoBehaviour
{

    Vector3 direction;
    float speed;
    float waitTime;
    int damage;
    float lifetime = 10f;
    Animator animator;
    [SerializeField] AnimationClip fireballAnim;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void setDirection(Vector3 newDir)
    {
        direction = newDir;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void setWaitTime(float newWaitTime)
    {
        waitTime = newWaitTime;
    }

    public void setDamage(int newDamage)
    {
        damage = newDamage;
    }

    void Update()
    {
        animator.Play(fireballAnim.name);
        updateWaitTime();
        updateLifetime();
        if (waitTime > 0)
        {
            return;
        }

        transform.position += direction*speed*Time.deltaTime;
    }

    private void updateWaitTime()
    {
        if (waitTime <= 0)
        {
            waitTime = 0;
        } else
        {
            waitTime -= Time.deltaTime;
        }
    }

    private void updateLifetime()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        Debug.Log(gameObject.name);
        EntityStatus es;
        Movement entityMovement;
        if (gameObject.tag == "Player")
        {
            es = gameObject.GetComponent<EntityStatus>();
            es.Hurt(damage);

            entityMovement = gameObject.GetComponent<Movement>();
            Vector2 dir = new Vector2(direction.x, direction.y).normalized;
            entityMovement.velocity += 0.05f * dir;
        } else
        {
            return;
        }
    }

}
