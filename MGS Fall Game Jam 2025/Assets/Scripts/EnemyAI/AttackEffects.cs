using Unity.VisualScripting;
using UnityEngine;

public class AttackEffects : MonoBehaviour
{
    private Vector2 direction;
    [SerializeField] BoxCollider2D boxCollider;
    private float speed;
    private int damage;
    private float duration;
    private float timeAlive = 0;
    private bool sharmadilloAttack;
    private GameObject sharm;

    public void setDirection(Vector2 dir)
    {
        direction = Vector2.Normalize(dir);
    }

    public void setDimensions(Vector2 dims)
    {
        boxCollider.size = dims;
    }

    public void setOffset(Vector2 offset)
    {
        if (direction == Vector2.right)
        {
            boxCollider.offset = offset;   
        } else
        {
            Vector2 newOffset = new Vector2(-offset.x, offset.y);
            boxCollider.offset = newOffset;
        }
    }

    public void setSpeed(float s)
    {
        speed = s;
    }

    public void setDamage(int d)
    {
        damage = d;
    }

    public void setDuration(float d)
    {
        duration = d;
    }

    public void setSharmadilloAttack(GameObject go)
    {
        sharmadilloAttack = true;
        sharm = go;
    }

    void Update()
    {
        if (direction == null)
        {
            return;
        }

        if (sharmadilloAttack)
        {
            transform.position = sharm.transform.position;
        } else
        {
            Vector3 vec3Dir = new Vector3(direction.x, direction.y, 0);
            transform.position += vec3Dir*speed*Time.deltaTime;   
        }
        
        timeAlive += Time.deltaTime;
        if (timeAlive > duration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D gameObject = collision.attachedRigidbody;
        EntityStatus es;
        if (gameObject.tag == "Player")
        {
            es = gameObject.GetComponent<EntityStatus>();
        } else
        {
            return;
        }

        es.Hurt(damage);
    }

}
