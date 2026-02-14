using UnityEngine;

public class PhoenixSlash : MonoBehaviour
{

    Vector3 direction;
    float speed;
    float waitTime;
    int damage;
    float lifetime = 10f;
    Animator animator;
    [SerializeField] AnimationClip slashAnim;

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
        animator.Play(slashAnim.name);
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
        if (gameObject.tag == "Player")
        {
            es = gameObject.GetComponent<EntityStatus>();
            es.Hurt(damage);
        } else
        {
            return;
        }
    }

}
